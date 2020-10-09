namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    // 0x8034
    public sealed record LeaveResponsePayload(byte Status) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType()}> Status: {Status:X2}";
    }
}
