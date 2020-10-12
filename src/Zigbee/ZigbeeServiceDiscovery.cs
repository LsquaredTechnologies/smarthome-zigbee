using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Lsquared.SmartHome.Zigbee.Extensibility;
using Lsquared.SmartHome.Zigbee.Protocol;

namespace Lsquared.SmartHome.Zigbee
{
    public sealed class ZigbeeServiceDiscovery : IExtension<ZigbeeNetwork>, IPayloadListener, ZDO.IDeviceListener
    {
        void IExtension<ZigbeeNetwork>.Attach([NotNull] ZigbeeNetwork network)
        {
            _network = network;
            _payloadUnsub = network.Subscribe((IPayloadListener)this);
            _deviceUnsub = network.Subscribe((ZDO.IDeviceListener)this);
        }

        void IExtension<ZigbeeNetwork>.Detach([NotNull] ZigbeeNetwork network)
        {
            _deviceUnsub?.Dispose();
            _payloadUnsub?.Dispose();
        }

        async void IPayloadListener.OnNext(ICommandPayload payload)
        {
            var task = payload switch
            {
                // ZDO: Service Discovery
                ZDO.GetActiveEndpointsResponsePayload p => HandleAsync(p),
                ZDO.GetNodeDescriptorResponsePayload p => HandleAsync(p),
                ZDO.GetPowerDescriptorResponsePayload p => HandleAsync(p),
                ZDO.GetSimpleDescriptorResponsePayload p => HandleAsync(p),
                ZDO.GetUserDescriptorResponsePayload p => HandleAsync(p),
                _ => default(ValueTask)
            };
            await task.ConfigureAwait(false);
        }

        async void ZDO.IDeviceListener.OnNext(INode node)
        {
            await _network.SendAsync(new ZDO.GetActiveEndpointsRequestPayload(node.NwkAddr)).ConfigureAwait(false);
            await _network.SendAsync(new ZDO.GetNodeDescriptorRequestPayload(node.NwkAddr)).ConfigureAwait(false);
            await _network.SendAsync(new ZDO.GetPowerDescriptorRequestPayload(node.NwkAddr)).ConfigureAwait(false);
        }

        private async ValueTask HandleAsync(ZDO.GetActiveEndpointsResponsePayload payload)
        {
            foreach (var endpoint in payload.ActiveEndpoints)
                await _network.SendAsync(new ZDO.GetSimpleDescriptorRequestPayload(payload.NwkAddr, endpoint)).ConfigureAwait(false);
        }

        private ValueTask HandleAsync(ZDO.GetNodeDescriptorResponsePayload payload)
        {
            if (_network!.Nodes.TryGetValue(payload.NwkAddr, out var node) &&
                node is Node n)
                n.NodeDescriptor = payload.NodeDescriptor;
            return default;
        }

        private ValueTask HandleAsync(ZDO.GetPowerDescriptorResponsePayload payload)
        {
            if (_network!.Nodes.TryGetValue(payload.NwkAddr, out var node) &&
                node is Node n)
                n.PowerDescriptor = payload.PowerDescriptor;
            return default;
        }

        private ValueTask HandleAsync(ZDO.GetSimpleDescriptorResponsePayload payload)
        {
            if (_network!.Nodes.TryGetValue(payload.NwkAddr, out var node) &&
                node is Node n &&
                n.Endpoints.Contains(payload.SimpleDescriptor.Endpoint) &&
                n.Endpoints[payload.SimpleDescriptor.Endpoint] is APP.NodeEndpoint ne)
                ne.Register(payload.SimpleDescriptor);
            return default;
        }

        private ValueTask HandleAsync(ZDO.GetUserDescriptorResponsePayload payload)
        {
            if (_network!.Nodes.TryGetValue(payload.NwkAddr, out var node) &&
                node is Node n)
                n.UserDescriptor = payload.UserDescriptor;
            return default;
        }

        private ZigbeeNetwork? _network;
        private IDisposable? _payloadUnsub;
        private IDisposable? _deviceUnsub;
    }
}
