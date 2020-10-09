using System;
using Lsquared.SmartHome.Buffers;

namespace Lsquared.SmartHome.Zigbee.Zigate
{
    // 0x0016
    public sealed record SetTimeRequestPayload(DateTimeOffset Time) : ICommandPayload
    {
        public void WriteTo(ref Span<byte> span, ref int offset, ref byte checksum)
        {
            var ts = (uint)(Time - Year2k).TotalSeconds;
            BigEndianBinary.Write(ts, ref span, ref offset, ref checksum);
        }
        private static readonly DateTimeOffset Year2k = new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero);
    }
}
