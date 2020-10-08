using System.Text;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record NodeDescriptor : IValue
    {
        public byte BitsFlag0 { get; init; }
        public byte BitsFlag1 { get; init; }
        public byte MacCapabilities { get; init; }
        public ushort ManufacturerCode { get; init; }
        public byte MaxBufferSize { get; init; }
        public ushort MaxRxSize { get; init; }
        public ushort ServerMask { get; init; }
        public ushort MaxTxSize { get; init; }
        public byte DescriptorCapabilities { get; init; }

        public override string ToString()
        {
            var b = new StringBuilder(1000);
            b.Append("ManufacturerCode: ").AppendFormat("{0:X4}", ManufacturerCode).Append(", ");
            b.Append("MaxBufferSize: ").AppendFormat("{0:X2}", MaxBufferSize).Append(", ");
            b.Append("MaxRxSize: ").AppendFormat("{0:X4}", MaxRxSize).Append(", ");
            b.Append("MaxTxSize: ").AppendFormat("{0:X4}", MaxTxSize).Append(", ");
            b.Append("ServerMask: ").AppendFormat("{0:X4}", ServerMask).Append(", ");
            b.Append("MacCapabilities: ").AppendFormat("{0:X2}", MacCapabilities).Append(", ");
            b.Append("DescriptorCapabilities: ").AppendFormat("{0:X2}", MacCapabilities);
            return b.ToString();
        }
    }
}
