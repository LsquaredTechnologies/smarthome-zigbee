using Lsquared.SmartHome.Zigbee.Protocol.Zigate;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record AddGroupRequest : Request
    {
        public AddGroupRequest(APP.Endpoint dstEndpoint, NWK.Address dstNwkAddr, NWK.GroupAddress grpAddr, string Name)
            : this(new Command<AddGroupRequestPayload>(
                new APP.Address(dstNwkAddr),
                dstEndpoint,
                dstEndpoint,
                new AddGroupRequestPayload(grpAddr, Name)))
        { }

        public AddGroupRequest(ICommand payload)
            : base(new ZigateCommandHeader(0x0060, 0), payload) { }
    }
}
