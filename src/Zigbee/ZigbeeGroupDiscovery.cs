using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Lsquared.SmartHome.Zigbee.Extensibility;
using Lsquared.SmartHome.Zigbee.Protocol;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups;

namespace Lsquared.SmartHome.Zigbee
{
    public sealed class ZigbeeGroupDiscovery : IExtension<ZigbeeNetwork>, IPayloadListener, ZCL.ICommandListener
    {
        public IReadOnlyCollection<NWK.GroupAddress> Groups =>
            _groups;

        void IExtension<ZigbeeNetwork>.Attach([NotNull] ZigbeeNetwork network)
        {
            _network = network;
            network.Subscribe((IPayloadListener)this);
            network.Subscribe((ZCL.ICommandListener)this);
        }

        void IExtension<ZigbeeNetwork>.Detach([NotNull] ZigbeeNetwork network) =>
            _network = null;

        async void IPayloadListener.OnNext(ICommandPayload payload)
        {
            var task = payload switch
            {
                ZDO.GetActiveEndpointsResponsePayload p => HandleAsync(p),
                ZCL.Command<GetGroupMembershipResponsePayload> c => HandleAsync(c),
                _ => default(ValueTask)
            };
            await task;
        }

        async void ZCL.ICommandListener.OnNext(ZCL.ICommand command)
        {
            var task = command switch
            {
                ZCL.Command<GetGroupMembershipResponsePayload> c => HandleAsync(c),
                _ => default(ValueTask)
            };
            await task;
        }

        private async ValueTask HandleAsync(ZDO.GetActiveEndpointsResponsePayload payload)
        {
            var nwkAddr = payload.NwkAddr;
            foreach (var endpoint in payload.ActiveEndpoints)
                await _network!.SendAsync(new ZCL.Command<GetGroupMembershipRequestPayload>(nwkAddr, 1, endpoint, new GetGroupMembershipRequestPayload()));
        }

        private ValueTask HandleAsync(ZCL.Command<GetGroupMembershipResponsePayload> payload)
        {
            foreach (var group in payload.Payload.Addresses)
                _groups.Add(group);
            return default;
        }

        private readonly CancellationTokenSource _stoppingCts = new();
        private readonly SortedSet<NWK.GroupAddress> _groups = new();
        private ZigbeeNetwork? _network;
    }
}
