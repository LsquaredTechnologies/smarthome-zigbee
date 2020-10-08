using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetSimpleDescriptorRequest : Request
    {
        public override ushort ExpectedResponseCode => 0x8043;

        public GetSimpleDescriptorRequest(NWK.Address nwkAddr, APP.Endpoint endpoint)
            : base(new ZigateCommandHeader(0x0043, 0), new GetSimpleDescriptorRequestPayload(nwkAddr, endpoint)) { }

        public GetSimpleDescriptorRequest(GetSimpleDescriptorRequestPayload payload)
            : base(new ZigateCommandHeader(0x0043, 0), payload) { }
    }
}
