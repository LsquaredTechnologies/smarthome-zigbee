using System;
using Lsquared.SmartHome.Buffers;

namespace Lsquared.SmartHome.Zigbee.Protocol.Zigate
{
    public sealed record ZigateCommandHeader(ushort CommandCode, ushort Length, byte Checksum = 0) : ICommandHeader
    {
        public void WriteTo(ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(CommandCode, ref span, ref offset, ref checksum);
            offset += 3; // skip length(2) and checksum(1)!
        }

        public override string ToString() =>
            $"Code: {CommandCode:X4}";
    }
}
