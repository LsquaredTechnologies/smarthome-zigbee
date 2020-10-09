namespace Lsquared.SmartHome.Zigbee.Zigate
{
    // 0x8000
    public sealed record StatusPayload(byte Status, byte Seq, ushort ResponseTo) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType().Name}> Status: {Status:X2}, Seq: {Seq:X2}, Command: {ResponseTo:X4}";
    }
}
