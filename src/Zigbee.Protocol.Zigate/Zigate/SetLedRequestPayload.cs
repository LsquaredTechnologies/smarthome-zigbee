using System;
using Lsquared.SmartHome.Buffers;

namespace Lsquared.SmartHome.Zigbee.Zigate
{
    // 0x0018
    public sealed class SetLedRequestPayload : ICommandPayload
    {
        public bool Status { get; } // use OnOff struct

        public SetLedRequestPayload(bool status) =>
            Status = status;

        public void WriteTo(ref Span<byte> span, ref int offset, ref byte checksum) =>
            BigEndianBinary.Write(Status, ref span, ref offset, ref checksum);
    }
}
