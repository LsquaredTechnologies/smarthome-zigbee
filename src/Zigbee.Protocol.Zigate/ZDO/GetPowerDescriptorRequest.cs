using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetPowerDescriptorRequest : Request
    {
        public override ushort ExpectedResponseCode => 0x8044;

        public GetPowerDescriptorRequest(NWK.Address nwkAddr)
            : this(new GetPowerDescriptorRequestPayload(nwkAddr)) { }

        public GetPowerDescriptorRequest(GetPowerDescriptorRequestPayload payload)
            : base(new ZigateCommandHeader(0x0044, 0), payload) { }
    }
}
