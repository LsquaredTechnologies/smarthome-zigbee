using Lsquared.SmartHome.Zigbee.Protocol.Zigate;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record ViewGroupRequest : Request
    {
        public ViewGroupRequest(APP.Endpoint dstEndpoint, NWK.Address dstNwkAddr, NWK.GroupAddress grpAddr)
            : base(
                  new ZigateCommandHeader(0x0061, 0),
                  new Command<AddGroupRequestPayload>(
                      new APP.Address(dstNwkAddr),
                      dstEndpoint,
                      dstEndpoint,
                      new AddGroupRequestPayload(grpAddr)))
        { }
    }
}
