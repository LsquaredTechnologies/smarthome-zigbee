using System.Text;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record NodeDescriptor
    {
        public LogicalType LogicalType => (LogicalType)(BitsFlag >> 5);

        public bool IsCoordinator => LogicalType == LogicalType.Coordinator;

        public bool IsRouter => LogicalType == LogicalType.Router;

        public bool IsEndDevice => LogicalType == LogicalType.EndDevice;

        public bool IsComplexDescriptorAvailable => (BitsFlag & 0b10000) == 0b10000;

        public bool IsUserDescriptorAvailable => (BitsFlag & 0b1000) == 0b1000;

        public ushort BitsFlag { get; init; }

        public MacCapabilities MacCapabilities { get; init; }

        public ushort ManufacturerCode { get; init; }

        public byte MaxBufferSize { get; init; }

        public ushort MaxRxSize { get; init; }

        public ushort ServerMask { get; init; }

        public ushort MaxTxSize { get; init; }

        public DescriptorCapabilities DescriptorCapabilities { get; init; }

        public override string ToString()
        {
            var b = new StringBuilder(1000);
            b.Append("ManufacturerCode: ").AppendFormat("{0:X4}", ManufacturerCode).Append(", ");
            b.Append("MaxBufferSize: ").AppendFormat("{0:X2}", MaxBufferSize).Append(", ");
            b.Append("MaxRxSize: ").AppendFormat("{0:X4}", MaxRxSize).Append(", ");
            b.Append("MaxTxSize: ").AppendFormat("{0:X4}", MaxTxSize).Append(", ");
            b.Append("ServerMask: ").AppendFormat("{0:X4}", ServerMask).Append(", ");
            b.Append("MacCapabilities: ").Append(MacCapabilities).Append(", ");
            b.Append("DescriptorCapabilities: ").Append(DescriptorCapabilities);
            return b.ToString();
        }
    }
}
