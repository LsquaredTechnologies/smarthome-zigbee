using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetExtendedAddressRequest : Request
    {
        public override ushort ExpectedResponseCode => 0x8041;

        public GetExtendedAddressRequest(NWK.Address nwkAddr, byte requestType, byte startIndex = 0)
            : base(new ZigateCommandHeader(0x0041, 0), new GetExtendedAddressRequestPayload(nwkAddr, requestType, startIndex)) { }

        public GetExtendedAddressRequest(GetExtendedAddressRequestPayload payload)
            : base(new ZigateCommandHeader(0x0041, 0), payload) { }
    }
}
