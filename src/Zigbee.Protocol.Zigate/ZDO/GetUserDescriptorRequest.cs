using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetUserDescriptorRequest : Request
    {
        public override ushort ExpectedResponseCode => 0x802C;

        public GetUserDescriptorRequest(NWK.Address nwkAddr)
            : this(new GetUserDescriptorRequestPayload(nwkAddr)) { }

        public GetUserDescriptorRequest(GetUserDescriptorRequestPayload payload)
            : base(new ZigateCommandHeader(0x002C, 0), payload) { }
    }
}
