using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    public sealed record GetNeighborTableRequest : Request
    {
        public override ushort ExpectedResponseCode => 0x804E;

        public GetNeighborTableRequest(NWK.Address dstExtAddr, byte startIndex)
            : base(new ZigateCommandHeader(0x004E, 0), new GetNeighborTableRequestPayload(dstExtAddr, startIndex)) { }

        public GetNeighborTableRequest(GetNeighborTableRequestPayload payload)
            : base(new ZigateCommandHeader(0x004E, 0), payload) { }
    }
}
