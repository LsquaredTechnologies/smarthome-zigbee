using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Lsquared.SmartHome.Zigbee.Extensibility;
using Lsquared.SmartHome.Zigbee.Protocol;

namespace Lsquared.SmartHome.Zigbee
{
    public sealed class ZigbeeNodeDiscovery : IExtension<ZigbeeNetwork>, IPayloadListener, ZDO.IDeviceListener
    {
        public IReadOnlyCollection<INode> Nodes =>
            _nodesByNwkAddr.Values;

        async void IExtension<ZigbeeNetwork>.Attach([NotNull] ZigbeeNetwork network)
        {
            _network = network;
            await StartAsync(network);
        }

        async void IExtension<ZigbeeNetwork>.Detach([NotNull] ZigbeeNetwork network) =>
            await StopAsync();

        private ValueTask StartAsync([NotNull] ZigbeeNetwork network)
        {
            ////var taskScheduler = new LimitedConcurrencyLevelTaskScheduler(2);
            ////var factory = new TaskFactory(taskScheduler);
            ////var task = factory.StartNew(() => ExecuteAsync(network, _stoppingCts.Token), _stoppingCts.Token);
            var task = ExecuteAsync(network, _stoppingCts.Token);
            if (task.IsCompleted) return new ValueTask(task);
            return default;
        }

        private ValueTask StopAsync()
        {
            _stoppingCts.Cancel();
            return default;
        }

        private async Task ExecuteAsync(ZigbeeNetwork network, CancellationToken stoppingToken)
        {
            await Task.Yield();

            using var payloadUnsubscriber = network.Subscribe((IPayloadListener)this);
            using var deviceUnsubscriber = network.Subscribe((ZDO.IDeviceListener)this);

            _commandPayloads.Add(new ZDO.Mgmt.GetRoutingTableRequestPayload(NWK.Address.Coordinator, 0));
            _commandPayloads.Add(new ZDO.Mgmt.GetNeighborTableRequestPayload(NWK.Address.Coordinator, 0));

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_commandPayloads.TryTake(out var requestPayload, 1000))
                    continue;

                await network.SendAsync(requestPayload);
            }

            _network = null;
        }

        async void IPayloadListener.OnNext(ICommandPayload payload)
        {
            var task = payload switch
            {
                ZDO.GetDevicesResponsePayload p => HandleAsync(p),
                ZDO.DeviceAnnounceIndicationPayload p => HandleAsync(p),
                ZDO.Mgmt.GetNeighborTableResponsePayload p => HandleAsync(p),
                ZDO.Mgmt.GetRoutingTableResponsePayload p => HandleAsync(p),
                ZDO.GetActiveEndpointsResponsePayload p => HandleAsync(p),
                ZDO.GetNodeDescriptorResponsePayload p => HandleAsync(p),
                ZDO.GetPowerDescriptorResponsePayload p => HandleAsync(p),
                ZDO.GetNwkAddressResponsePayload p => HandleAsync(p),
                ZDO.GetExtendedAddressResponsePayload p => HandleAsync(p),
                ZDO.GetSimpleDescriptorResponsePayload p => HandleAsync(p),
                ZDO.GetUserDescriptorResponsePayload p => HandleAsync(p),
                _ => default(ValueTask)
            };
            await task;
        }

        void ZDO.IDeviceListener.OnNext(INode node)
        {
            _nodesByNwkAddr.TryAdd(node.NwkAddr, node);
            _commandPayloads.Add(new ZDO.GetActiveEndpointsRequestPayload(node.NwkAddr));
            _commandPayloads.Add(new ZDO.GetNodeDescriptorRequestPayload(node.NwkAddr));
            _commandPayloads.Add(new ZDO.GetPowerDescriptorRequestPayload(node.NwkAddr));
        }

        private ValueTask HandleAsync(ZDO.GetDevicesResponsePayload payload)
        {
            return default;
        }

        private ValueTask HandleAsync(ZDO.DeviceAnnounceIndicationPayload payload)
        {
            ////_network!.RegisterNode(payload.ExtAddr, payload.NwkAddr);
            return default;
        }

        private async ValueTask HandleAsync(ZDO.Mgmt.GetNeighborTableResponsePayload payload)
        {
            var n = (byte)(payload.StartIndex + payload.NeighborTableEntries.Count);
            if (payload.NeighborTableEntries.Count > 0)
            {
                //var nwkAddr = payload.NeighborTableEntries[0].NwkAddr;
                //_commandPayloads.Add(new ZDO.Mgmt.GetNeighborTableRequestPayload(nwkAddr, n));

                foreach (var neighbor in payload.NeighborTableEntries)
                {
                    var resp = await _network!.SendAndReceiveAsync(new ZDO.GetNwkAddressRequestPayload(neighbor.ExtAddr, 0, 0));
                    if (resp is ZDO.GetNwkAddressResponsePayload p)
                        if (_network.GetNode(p.NwkAddr) is Node node && node is not null && !node.IsEndDevice)
                            _commandPayloads.Add(new ZDO.Mgmt.GetNeighborTableRequestPayload(node.NwkAddr, 0));
                }
            }
        }

        private ValueTask HandleAsync(ZDO.Mgmt.GetRoutingTableResponsePayload payload)
        {
            var n = (byte)(payload.StartIndex + payload.RoutingTableEntries.Count);
            if (payload.RoutingTableEntries.Count > 0 && payload.Capacity > n)
            {
                //var nwkAddr = payload.RoutingTableEntries[0].NwkAddr;
                //_commandPayloads.Add(new ZDO.Mgmt.GetRoutingTableRequestPayload(nwkAddr, n));
            }
            return default;
        }

        private ValueTask HandleAsync(ZDO.GetActiveEndpointsResponsePayload payload)
        {
            foreach (var endpoint in payload.ActiveEndpoints)
                // TODO how to register endpoint in node?
                _commandPayloads.Add(new ZDO.GetSimpleDescriptorRequestPayload(payload.NwkAddr, endpoint));
            return default;
        }

        private ValueTask HandleAsync(ZDO.GetNodeDescriptorResponsePayload payload)
        {
            if (_nodesByNwkAddr.TryGetValue(payload.NwkAddr, out var node))
                ((Node)node).Info = payload.NodeDescriptor;

            return default;
        }

        private ValueTask HandleAsync(ZDO.GetPowerDescriptorResponsePayload payload)
        {
            if (_nodesByNwkAddr.TryGetValue(payload.NwkAddr, out var node))
                ((Node)node).PowerInfo = payload.PowerDescriptor;
            return default;
        }

        private ValueTask HandleAsync(ZDO.GetNwkAddressResponsePayload payload)
        {
            return default;
        }

        private ValueTask HandleAsync(ZDO.GetExtendedAddressResponsePayload payload)
        {
            return default;
        }

        private ValueTask HandleAsync(ZDO.GetSimpleDescriptorResponsePayload payload)
        {
            if (_nodesByNwkAddr.TryGetValue(payload.NwkAddr, out var node) && node.Endpoints.Contains(payload.SimpleDescriptor.Endpoint))
                ((APP.NodeEndpoint)node.Endpoints[payload.SimpleDescriptor.Endpoint]).Register(payload.SimpleDescriptor);
            return default;
        }

        private ValueTask HandleAsync(ZDO.GetUserDescriptorResponsePayload payload)
        {
            if (_nodesByNwkAddr.TryGetValue(payload.NwkAddr, out var node))
                ((Node)node).UserInfo = payload.UserDescriptor;
            return default;
        }

        private readonly CancellationTokenSource _stoppingCts = new();
        private readonly BlockingCollection<ICommandPayload> _commandPayloads = new();
        private readonly Dictionary<NWK.Address, INode> _nodesByNwkAddr = new();
        private ZigbeeNetwork? _network;
    }
}
