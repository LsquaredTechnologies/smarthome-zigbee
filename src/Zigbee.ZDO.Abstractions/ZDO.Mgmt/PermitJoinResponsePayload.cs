namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    // 0x8036
    public sealed record PermitJoinResponsePayload(byte Status) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType()}> Status: {Status:X2}";
    }
}
