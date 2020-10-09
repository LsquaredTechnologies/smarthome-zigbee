namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record CommandHeader(byte CommandCode) : ICommandHeader
    {
        public byte Control { get; init; }

        public ushort? ManufacturerCode { get; init; }

        public byte Seq { get; init; }
    }
}
