using System;

namespace Lsquared.SmartHome.Zigbee
{
    public sealed class NoCommandPayload : ICommandPayload
    {
        internal NoCommandPayload() { }

        ////void ICommandPayload.WriteTo(ref Span<byte> span, ref int offset, ref byte checksum) { }
    }
}
