using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetActiveEndpointsRequest : Request
    {
        public override ushort ExpectedResponseCode => 0x8045;

        public GetActiveEndpointsRequest(NWK.Address nwkAddr)
            : base(new ZigateCommandHeader(0x0045, 0), new GetActiveEndpointsRequestPayload(nwkAddr)) { }

        public GetActiveEndpointsRequest(GetActiveEndpointsRequestPayload payload)
            : base(new ZigateCommandHeader(0x0045, 0), payload) { }
    }
}
