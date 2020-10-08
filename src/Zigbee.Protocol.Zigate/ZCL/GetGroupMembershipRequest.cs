using System.Collections.Generic;
using Lsquared.SmartHome.Zigbee.Protocol.Zigate;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record GetGroupMembershipRequest : Request
    {
        public GetGroupMembershipRequest(APP.Endpoint dstEndpoint, NWK.Address dstNwkAddr)
            : this(new Command<GetGroupMembershipRequestPayload>(
                new APP.Address(dstNwkAddr),
                1,
                dstEndpoint,
                new GetGroupMembershipRequestPayload()))
        { }

        public GetGroupMembershipRequest(APP.Endpoint dstEndpoint, NWK.Address dstNwkAddr, IEnumerable<NWK.GroupAddress> grpAddrs)
            : this(new Command<GetGroupMembershipRequestPayload>(
                new APP.Address(dstNwkAddr),
                1,
                dstEndpoint,
                new GetGroupMembershipRequestPayload(new Array<NWK.GroupAddress>(grpAddrs))))
            { }

        public GetGroupMembershipRequest(Command<GetGroupMembershipRequestPayload> payload)
            : base(new ZigateCommandHeader(0x0062, 0), payload) { }
    }
}
