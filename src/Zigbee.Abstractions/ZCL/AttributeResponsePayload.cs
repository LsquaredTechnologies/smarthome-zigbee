namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record AttributeResponsePayload(ushort ID, byte Flags, AttributeValue Value) : Zigbee.ICommandPayload
    {
        public override string ToString() =>
            $"- ID: {ID:X4}, Flags: {Flags:X2}, {Value}";
    }
}
