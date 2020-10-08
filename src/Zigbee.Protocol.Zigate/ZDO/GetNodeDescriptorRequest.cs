using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetNodeDescriptorRequest : Request
    {
        public override ushort ExpectedResponseCode => 0x8042;

        public GetNodeDescriptorRequest(NWK.Address nwkAddr)
            : base(new ZigateCommandHeader(0x0042, 0), new GetNodeDescriptorRequestPayload(nwkAddr)) { }

        public GetNodeDescriptorRequest(GetNodeDescriptorRequestPayload payload)
            : base(new ZigateCommandHeader(0x0042, 0), payload) { }
    }
}
