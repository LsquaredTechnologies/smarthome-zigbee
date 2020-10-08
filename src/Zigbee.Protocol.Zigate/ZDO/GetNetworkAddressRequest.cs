using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetNetworkAddressRequest : Request
    {
        public override ushort ExpectedResponseCode => 0x8040;

        public GetNetworkAddressRequest(MAC.Address extAddr, byte requestType, byte startIndex = 0)
            : base(new ZigateCommandHeader(0x0040, 0), new GetNetworkAddressRequestPayload(extAddr, requestType, startIndex)) { }

        public GetNetworkAddressRequest(GetNetworkAddressRequestPayload payload)
            : base(new ZigateCommandHeader(0x0040, 0), payload) { }
    }
}
