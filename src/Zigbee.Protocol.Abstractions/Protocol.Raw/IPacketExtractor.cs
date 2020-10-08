using System;
using System.Buffers;

namespace Lsquared.SmartHome.Zigbee.Protocol.Raw
{
    public interface IPacketExtractor
    {
        bool TryExtract(ReadOnlySequence<byte> buffer, out ReadOnlyMemory<byte> packet, out SequencePosition examined, out SequencePosition consumed);
    }
}
