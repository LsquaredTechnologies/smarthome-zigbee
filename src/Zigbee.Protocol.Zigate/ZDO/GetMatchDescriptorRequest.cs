using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetMatchDescriptorRequest : Request
    {
        public override ushort ExpectedResponseCode => 0x8046;

        public GetMatchDescriptorRequest(NWK.Address nwkAddr, ushort ProfileID, Array<ushort> InputClusters, Array<ushort> OutputClusters)
            : base(new ZigateCommandHeader(0x0046, 0), new GetMatchDescriptorRequestPayload(nwkAddr, ProfileID, InputClusters, OutputClusters)) { }

        public GetMatchDescriptorRequest(GetMatchDescriptorRequestPayload payload)
            : base(new ZigateCommandHeader(0x0046, 0), payload) { }
    }
}
