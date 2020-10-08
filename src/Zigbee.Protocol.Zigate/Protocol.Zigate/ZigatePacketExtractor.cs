using System;
using System.Buffers;
using System.Diagnostics;
using Lsquared.SmartHome.Zigbee.Protocol.Raw;

namespace Lsquared.SmartHome.Zigbee.Protocol.Zigate
{
    internal sealed class ZigatePacketExtractor : IPacketExtractor
    {
        public bool TryExtract(ReadOnlySequence<byte> buffer, out ReadOnlyMemory<byte> packet, out SequencePosition examined, out SequencePosition consumed)
        {
            packet = default;
            examined = buffer.End;
            consumed = buffer.GetPosition(0);

            var endPosition = buffer.PositionOf<byte>(0x03);
            if (!endPosition.HasValue)
                // no separator found, try to read more bytes...
                return false;

            Debug.WriteLine("[DEBUG] Packet read.");

            // include the separator into the current reading packet!
            consumed = buffer.GetPosition(1, endPosition.Value);

            packet = buffer.Slice(0, consumed).First.ToArray();

            if (packet.Length < 5 || packet.Span[0] != 0x01 || packet.Span[^1] != 0x03)
            {
                Debug.WriteLine("[ERROR] Invalid packet!");
                return false;
            }

            return true;
        }
    }
}
