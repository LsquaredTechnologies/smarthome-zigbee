using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record RemoveAllGroupsRequest : Request
    {
        public RemoveAllGroupsRequest(APP.Endpoint dstEndpoint, NWK.Address dstNwkAddr)
            : base(
                  new ZigateCommandHeader(0x0064, 0),
                  new Command(
                      new APP.Address(dstNwkAddr),
                      dstEndpoint,
                      dstEndpoint))
        { }
    }
}
