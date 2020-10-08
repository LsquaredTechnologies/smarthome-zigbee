namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    // 0x0036
    public sealed record PermitJoinRequestPayload(NWK.Address DstNwkAddr, byte Duration, byte TrustCenterSignificance = 0) : ICommandPayload
    {
        public PermitJoinRequestPayload(NWK.Address DstNwkAddr, bool enable) : this(DstNwkAddr, enable ? (byte)0xFF : (byte)0, 0) { }
    }
}
