using Lsquared.SmartHome.Zigbee.Protocol.Zigate;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record RemoveGroupRequest : Request
    {
        public RemoveGroupRequest(APP.Endpoint dstEndpoint, NWK.Address dstNwkAddr, NWK.GroupAddress grpAddr)
            : base(
                  new ZigateCommandHeader(0x0063, 0),
                  new Command<AddGroupRequestPayload>(
                      new APP.Address(dstNwkAddr),
                      dstEndpoint,
                      dstEndpoint,
                      new AddGroupRequestPayload(grpAddr)))
        { }
    }
}
