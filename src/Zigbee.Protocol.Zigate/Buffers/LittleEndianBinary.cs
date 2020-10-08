using System;

namespace Lsquared.SmartHome.Buffers
{
    public static class LittleEndianBinary
    {
        public static void Write(bool value, ref Span<byte> span, ref int offset, ref byte checksum) =>
            checksum ^= span[offset++] = value ? (byte)1 : (byte)0;

        public static void Write(byte value, ref Span<byte> span, ref int offset, ref byte checksum) =>
            checksum ^= span[offset++] = value;

        public static void Write(ushort value, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            checksum ^= span[offset++] = (byte)(value >> 0);
            checksum ^= span[offset++] = (byte)(value >> 8);
        }

        public static void Write(uint value, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            checksum ^= span[offset++] = (byte)(value >> 0);
            checksum ^= span[offset++] = (byte)(value >> 8);
            checksum ^= span[offset++] = (byte)(value >> 16);
            checksum ^= span[offset++] = (byte)(value >> 24);
        }

        public static void Write(ulong value, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            checksum ^= span[offset++] = (byte)(value >> 0);
            checksum ^= span[offset++] = (byte)(value >> 8);
            checksum ^= span[offset++] = (byte)(value >> 16);
            checksum ^= span[offset++] = (byte)(value >> 24);
            checksum ^= span[offset++] = (byte)(value >> 32);
            checksum ^= span[offset++] = (byte)(value >> 40);
            checksum ^= span[offset++] = (byte)(value >> 48);
            checksum ^= span[offset++] = (byte)(value >> 56);
        }

        public static bool ReadBoolean(ref ReadOnlySpan<byte> span, ref int offset) =>
            span[offset++] == 1;

        public static byte ReadByte(ref ReadOnlySpan<byte> span, ref int offset) =>
            span[offset++];

        public static ushort ReadUInt16(ref ReadOnlySpan<byte> span, ref int offset) =>
            (ushort)(span[offset++] | (span[offset++] << 8));

        public static ushort ReadUInt16(ReadOnlyMemory<byte> buffer, int offset) =>
            (ushort)(buffer.Span[offset++] | (buffer.Span[offset++] << 8));

        public static uint ReadUInt32(ref ReadOnlySpan<byte> span, ref int offset) =>
            (uint)(span[offset++] | (span[offset++] << 8) | (span[offset++] << 16) | (span[offset++] << 24));

        public static ulong ReadUInt64(ref ReadOnlySpan<byte> span, ref int offset) =>
            (ulong)ReadUInt32(ref span, ref offset) | ((ulong)ReadUInt32(ref span, ref offset) << 32);
    }
}
