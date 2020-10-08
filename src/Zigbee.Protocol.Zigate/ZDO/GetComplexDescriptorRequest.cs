using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetComplexDescriptorRequest : Request
    {
        public override ushort ExpectedResponseCode => 0x8531;

        public GetComplexDescriptorRequest(NWK.Address nwkAddr)
            : base(new ZigateCommandHeader(0x0531, 0), new GetComplexDescriptorRequestPayload(nwkAddr)) { }

        public GetComplexDescriptorRequest(GetComplexDescriptorRequestPayload payload)
            : base(new ZigateCommandHeader(0x0531, 0), payload) { }
    }
}
