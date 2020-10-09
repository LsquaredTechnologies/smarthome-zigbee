using System;
using System.Text;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record AttributeValue(byte DataType, ushort Length, byte[] Raw) : Zigbee.ICommandPayload
    {
        private byte[] ReverseRaw
        {
            get
            {
                var x = new byte[Length];
                Array.Copy(Raw, x, Length);
                Array.Reverse(x);
                return x;
            }
        }
        public override string ToString() => DataType switch
        {
            0x00 => $"DataType: {DataType:X2}, <No Value> [Length: {Length}]",

            0x08 => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")} [Length: {Length}]",
            0x09 => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")} [Length: {Length}]",
            0x0A => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")} [Length: {Length}]",
            0x0B => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")} [Length: {Length}]",
            0x0C => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")} [Length: {Length}]",
            0x0D => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")} [Length: {Length}]",
            0x0E => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")} [Length: {Length}]",
            0x0F => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")} [Length: {Length}]",

            0x10 => $"DataType: {DataType:X2}, Value: {Raw[0] == 1} [Length: {Length}]",

            0x18 => $"DataType: {DataType:X2}, Raw: 0b{Convert.ToString(Raw[0], 2).PadLeft(8, '0')} [Length: {Length}]",
            0x19 => $"DataType: {DataType:X2}, Raw: 0b{Convert.ToString(BitConverter.ToInt16(ReverseRaw), 2).PadLeft(16, '0')} [Length: {Length}]",
            0x1A => $"DataType: {DataType:X2}, Raw: 0b{Convert.ToString(BitConverter.ToInt32(ReverseRaw), 2).PadLeft(24, '0')} [Length: {Length}]",
            0x1B => $"DataType: {DataType:X2}, Raw: 0b{Convert.ToString(BitConverter.ToInt32(ReverseRaw), 2).PadLeft(32, '0')} [Length: {Length}]",
            0x1C => $"DataType: {DataType:X2}, Raw: 0b{Convert.ToString(BitConverter.ToInt64(ReverseRaw), 2).PadLeft(40, '0')} [Length: {Length}]",
            0x1D => $"DataType: {DataType:X2}, Raw: 0b{Convert.ToString(BitConverter.ToInt64(ReverseRaw), 2).PadLeft(48, '0')} [Length: {Length}]",
            0x1E => $"DataType: {DataType:X2}, Raw: 0b{Convert.ToString(BitConverter.ToInt64(ReverseRaw), 2).PadLeft(56, '0')} [Length: {Length}]",
            0x1F => $"DataType: {DataType:X2}, Raw: 0b{Convert.ToString(BitConverter.ToInt64(ReverseRaw), 2).PadLeft(64, '0')} [Length: {Length}]",

            0x20 => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")}, Value: {Raw[0]} [Length: {Length}]",
            0x21 => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")}, Value: {BitConverter.ToUInt16(ReverseRaw)} [Length: {Length}]",
            0x22 => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")}, Value: {BitConverter.ToUInt32(ReverseRaw)} [Length: {Length}]",
            0x23 => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")}, Value: {BitConverter.ToUInt32(ReverseRaw)} [Length: {Length}]",
            0x24 => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")}, Value: {BitConverter.ToUInt64(ReverseRaw)} [Length: {Length}]",
            0x25 => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")}, Value: {BitConverter.ToUInt64(ReverseRaw)} [Length: {Length}]",
            0x26 => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")}, Value: {BitConverter.ToUInt64(ReverseRaw)} [Length: {Length}]",
            0x27 => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")}, Value: {BitConverter.ToUInt64(ReverseRaw)} [Length: {Length}]",

            0x28 => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")}, Value: {Raw[0]} [Length: {Length}]",
            0x29 => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")}, Value: {BitConverter.ToInt16(ReverseRaw)} [Length: {Length}]",
            0x2A => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")}, Value: {BitConverter.ToInt32(ReverseRaw)} [Length: {Length}]",
            0x2B => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")}, Value: {BitConverter.ToInt32(ReverseRaw)} [Length: {Length}]",
            0x2C => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")}, Value: {BitConverter.ToInt64(ReverseRaw)} [Length: {Length}]",
            0x2D => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")}, Value: {BitConverter.ToInt64(ReverseRaw)} [Length: {Length}]",
            0x2E => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")}, Value: {BitConverter.ToInt64(ReverseRaw)} [Length: {Length}]",
            0x2F => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")}, Value: {BitConverter.ToInt64(ReverseRaw)} [Length: {Length}]",

            0x30 => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")} [Length: {Length}]",
            0x31 => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")} [Length: {Length}]",

            0x38 => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")} Value: {BitConverter.ToSingle(ReverseRaw)} [Length: {Length}]",
            0x39 => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")} Value: {BitConverter.ToSingle(ReverseRaw)} [Length: {Length}]",
            0x3A => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")} Value: {BitConverter.ToDouble(ReverseRaw)} [Length: {Length}]",

            0x41 => $"DataType: {DataType:X2}, String: {Encoding.ASCII.GetString(Raw).Replace("\n", " ")} (0x{BitConverter.ToString(Raw).Replace("-", "")}) [Length: {Length}]",
            0x42 => $"DataType: {DataType:X2}, String: {Encoding.ASCII.GetString(Raw).Replace("\n", " ")} (0x{BitConverter.ToString(Raw).Replace("-", "")}) [Length: {Length}]",
            0x43 => $"DataType: {DataType:X2}, String: {Encoding.Unicode.GetString(Raw).Replace("\n", " ")} (0x{BitConverter.ToString(Raw).Replace("-", "")}) [Length: {Length}]",
            0x44 => $"DataType: {DataType:X2}, String: {Encoding.Unicode.GetString(Raw).Replace("\n", " ")} (0x{BitConverter.ToString(Raw).Replace("-", "")}) [Length: {Length}]",

            0x48 => $"DataType: {DataType:X2}, Array: [{BitConverter.ToString(Raw).Replace("-", ",")}] [Length: {Length}]",
            0x4C => $"DataType: {DataType:X2}, Array: [{BitConverter.ToString(Raw).Replace("-", ",")}] [Length: {Length}]",

            _ => $"DataType: {DataType:X2}, Raw: 0x{BitConverter.ToString(Raw).Replace("-", "")} [Length: {Length}]",
        };
    }
}
