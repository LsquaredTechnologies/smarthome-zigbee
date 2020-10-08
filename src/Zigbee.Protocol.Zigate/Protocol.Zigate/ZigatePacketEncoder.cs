using System;
using System.Buffers;
using Lsquared.SmartHome.Zigbee.Protocol.Raw;

namespace Lsquared.SmartHome.Zigbee.Protocol.Zigate
{
    internal sealed class ZigatePacketEncoder : IPacketEncoder
    {
        public ReadOnlyMemory<byte> Decode(ReadOnlyMemory<byte> raw)
        {
            // decode without allocating memory (thx to MemoryPool)
            using var decodedPacketMemory = MemoryPool<byte>.Shared.Rent(raw.Length);
            var decodedPacket = decodedPacketMemory.Memory;

            var readOffset = 0;
            var writeOffset = 0;
            readOffset++; // eat 0x01
            while (readOffset < raw.Length && raw.Span[readOffset] != 0x03)
            {
                var @byte = raw.Span[readOffset++];
                if (@byte == 0x02)
                    @byte = (byte)(raw.Span[readOffset++] ^ 0x10);
                decodedPacket.Span[writeOffset++] = @byte;
            }
            //readOffset++; // eat 0x03

            decodedPacket = decodedPacket.Slice(0, writeOffset);

            // return a copy of decoded packet (memory allocation occurs)
            return decodedPacket.ToArray();
        }

        public ReadOnlyMemory<byte> Encode(ReadOnlyMemory<byte> raw)
        {
            // encode without allocating memory (thx to MemoryPool)
            using var encodedPacketRent = MemoryPool<byte>.Shared.Rent(2 + 2 * raw.Length);
            var encodedPacket = encodedPacketRent.Memory;

            var readOffset = 0;
            var readSpan = raw.Span;

            var writeOffset = 0;
            var writeSpan = encodedPacket.Span;

            writeSpan[writeOffset++] = 0x01;
            while (readOffset < raw.Length)
            {
                var @byte = readSpan[readOffset++];
                if (@byte < 0x10)
                {
                    @byte ^= 0x10;
                    writeSpan[writeOffset++] = 0x02;
                }
                writeSpan[writeOffset++] = @byte;
            }
            writeSpan[writeOffset++] = 0x03;

            // return a copy of encoded packet (memory allocation occurs)
            return encodedPacket.Slice(0, writeOffset).ToArray();
        }
    }
}
