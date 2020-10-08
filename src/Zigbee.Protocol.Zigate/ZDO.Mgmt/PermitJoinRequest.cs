using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    public sealed record PermitJoinRequest : Request
    {
        public override ushort ExpectedResponseCode => 0x8049;

        public PermitJoinRequest(NWK.Address dstNwkAddr, bool join, byte trustCenterSignificance = 0)
            : base(new ZigateCommandHeader(0x0049, 0), new PermitJoinRequestPayload(dstNwkAddr, join ? (byte)0xFF : (byte)0, trustCenterSignificance)) { }

        public PermitJoinRequest(NWK.Address dstNwkAddr, byte duration, byte trustCenterSignificance = 0)
            : base(new ZigateCommandHeader(0x0049, 0), new PermitJoinRequestPayload(dstNwkAddr, duration, trustCenterSignificance)) { }

        public PermitJoinRequest(PermitJoinRequestPayload payload)
            : base(new ZigateCommandHeader(0x0049, 0), payload) { }
    }
}
