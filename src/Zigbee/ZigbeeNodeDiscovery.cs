using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Lsquared.SmartHome.Zigbee.Extensibility;
using Lsquared.SmartHome.Zigbee.Protocol;

namespace Lsquared.SmartHome.Zigbee
{
    public sealed class ZigbeeNodeDiscovery : IExtension<ZigbeeNetwork>, IPayloadListener, ZDO.IDeviceListener
    {
        async void IExtension<ZigbeeNetwork>.Attach([NotNull] ZigbeeNetwork network)
        {
            _network = network;
            _payloadUnsub = network.Subscribe((IPayloadListener)this);
            _deviceUnsub = network.Subscribe((ZDO.IDeviceListener)this);
            await StartAsync(network).ConfigureAwait(false);
        }

        async void IExtension<ZigbeeNetwork>.Detach([NotNull] ZigbeeNetwork network)
        {
            _deviceUnsub?.Dispose();
            _payloadUnsub?.Dispose();
            await StopAsync().ConfigureAwait(false);
        }

        private ValueTask StartAsync([NotNull] ZigbeeNetwork network)
        {
            var task = ExecuteAsync(_stoppingCts.Token);
            if (task.IsCompleted) return new ValueTask(task);
            return default;
        }

        private ValueTask StopAsync()
        {
            _stoppingCts.Cancel();
            return default;
        }

        private async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1500).ConfigureAwait(false);
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_queue.TryTake(out var nwkAddr, 50))
                    continue;

                await _network!.SendAsync(new ZDO.Mgmt.GetNeighborTableRequestPayload(nwkAddr, 0)).ConfigureAwait(false);
                _queue.Add(nwkAddr);
                await Task.Delay(30000).ConfigureAwait(false);
            }

            _network = null;
        }

        async void IPayloadListener.OnNext(ICommandPayload payload)
        {
            var task = payload switch
            {
                // ZDO: Device Discovery
                ////ZDO.DeviceAnnounceIndicationPayload p => HandleAsync(p),
                ZDO.GetDevicesResponsePayload p => HandleAsync(p),
                ZDO.GetNetworkAddressResponsePayload p => HandleAsync(p),
                ZDO.GetExtendedAddressResponsePayload p => HandleAsync(p),
                // ZDO: Management
                ZDO.Mgmt.GetNeighborTableResponsePayload p => HandleAsync(p),
                _ => default(ValueTask)
            };
            await task.ConfigureAwait(false);
        }

        void ZDO.IDeviceListener.OnNext(INode node)
        {
            _nodesByNwkAddr.TryAdd(node.NwkAddr, node.ExtAddr);
            _nodesByExtAddr.TryAdd(node.ExtAddr, node.NwkAddr);
        }

        private ValueTask HandleAsync(ZDO.GetDevicesResponsePayload payload)
        {
            foreach (var device in payload.Devices)
            {
                _nodesByNwkAddr.TryAdd(device.NwkAddr, device.ExtAddr);
                _nodesByExtAddr.TryAdd(device.ExtAddr, device.NwkAddr);
            }
            return default;
        }

        ////private ValueTask HandleAsync(ZDO.DeviceAnnounceIndicationPayload payload)
        ////{
        ////    _nodesByNwkAddr.TryAdd(payload.NwkAddr, payload.ExtAddr);
        ////    _nodesByExtAddr.TryAdd(payload.ExtAddr, payload.NwkAddr);
        ////    return default;
        ////}

        private async ValueTask HandleAsync(ZDO.Mgmt.GetNeighborTableResponsePayload payload)
        {
            var n = (byte)(payload.StartIndex + payload.NeighborTableEntries.Count);
            if (payload.NeighborTableEntries.Count > 0)
            {
                if (payload.StartIndex + payload.NeighborTableEntries.Count < payload.Capacity)
                    await _network!.SendAsync(new ZDO.Mgmt.GetNeighborTableRequestPayload(payload.SrcNwkAddr, n)).ConfigureAwait(false);

                foreach (var neighbor in payload.NeighborTableEntries)
                {
                    var resp = await _network!.SendAndReceiveAsync(new ZDO.GetNetworkAddressRequestPayload(neighbor.ExtAddr, 0, 0)).ConfigureAwait(false);
                    if (resp is ZDO.GetNetworkAddressResponsePayload p && p.NwkAddr != NWK.Address.Invalid)
                    {
                        if (_nodesByNwkAddr.TryAdd(p.NwkAddr, p.ExtAddr))
                            _queue.Add(p.NwkAddr);
                        _nodesByExtAddr.TryAdd(p.ExtAddr, p.NwkAddr);
                        await _network!.SendAsync(new ZDO.Mgmt.GetNeighborTableRequestPayload(p.NwkAddr, 0)).ConfigureAwait(false);
                    }
                }
            }
        }

        private ValueTask HandleAsync(ZDO.GetNetworkAddressResponsePayload payload)
        {
            if (payload.NwkAddr != NWK.Address.Invalid)
            {
                if (_nodesByNwkAddr.TryAdd(payload.NwkAddr, payload.ExtAddr))
                    _queue.Add(payload.NwkAddr);
                _nodesByExtAddr.TryAdd(payload.ExtAddr, payload.NwkAddr);
            }
            return default;
        }

        private async ValueTask HandleAsync(ZDO.GetExtendedAddressResponsePayload payload)
        {
            if (_nodesByNwkAddr.TryAdd(payload.NwkAddr, payload.ExtAddr))
                _queue.Add(payload.NwkAddr);
            _nodesByExtAddr.TryAdd(payload.ExtAddr, payload.NwkAddr);
            if (payload.NwkAddr == NWK.Address.Coordinator)
                await _network!.SendAsync(new ZDO.Mgmt.GetNeighborTableRequestPayload(NWK.Address.Coordinator, 0)).ConfigureAwait(false);
        }

        private readonly CancellationTokenSource _stoppingCts = new();
        private readonly Dictionary<NWK.Address, MAC.Address> _nodesByNwkAddr = new();
        private readonly Dictionary<MAC.Address, NWK.Address> _nodesByExtAddr = new();
        private readonly BlockingCollection<NWK.Address> _queue = new();
        private ZigbeeNetwork? _network;
        private IDisposable? _payloadUnsub;
        private IDisposable? _deviceUnsub;
    }
}
