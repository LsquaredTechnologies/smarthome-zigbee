namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    // 0x8038
    public sealed record NetworkUpdateResponsePayload(byte Status) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType()}> Status: {Status:X2}";
    }
}
