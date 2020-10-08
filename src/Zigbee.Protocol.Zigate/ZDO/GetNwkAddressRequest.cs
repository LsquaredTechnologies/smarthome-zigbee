using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetNwkAddressRequest : Request
    {
        public override ushort ExpectedResponseCode => 0x8040;

        public GetNwkAddressRequest(MAC.Address extAddr, byte requestType, byte startIndex = 0)
            : base(new ZigateCommandHeader(0x0040, 0), new GetNwkAddressRequestPayload(extAddr, requestType, startIndex)) { }

        public GetNwkAddressRequest(GetNwkAddressRequestPayload payload)
            : base(new ZigateCommandHeader(0x0040, 0), payload) { }
    }
}
