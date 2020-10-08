using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record IdentifyQueryRequest : Request
    {
        public IdentifyQueryRequest(NWK.Address dstNwkAddr)
            : this(dstNwkAddr, 1) { }

        public IdentifyQueryRequest(NWK.Address dstNwkAddr, APP.Endpoint dstEndpoint)
            : base(
                  new ZigateCommandHeader(0x0071, 0),
                  new Command(
                      new APP.Address(dstNwkAddr),
                      dstEndpoint,
                      dstEndpoint))
        { }
    }
}
