using System;
using static Lsquared.Extensions.Functional;

namespace Lsquared.SmartHome.Buffers
{
    public static class BigEndianBinary
    {
        public static Unit Write(bool value, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            checksum ^= span[offset++] = value ? (byte)1 : (byte)0;
            return default;
        }

        public static Unit Write(byte value, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            checksum ^= span[offset++] = value;
            return default;
        }

            public static Unit Write(ushort value, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            checksum ^= span[offset++] = (byte)(value >> 8);
            checksum ^= span[offset++] = (byte)(value >> 0);
            return default;
        }

        public static Unit Write(uint value, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            checksum ^= span[offset++] = (byte)(value >> 24);
            checksum ^= span[offset++] = (byte)(value >> 16);
            checksum ^= span[offset++] = (byte)(value >> 8);
            checksum ^= span[offset++] = (byte)(value >> 0);
            return default;
        }

        public static Unit Write(ulong value, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            checksum ^= span[offset++] = (byte)(value >> 56);
            checksum ^= span[offset++] = (byte)(value >> 48);
            checksum ^= span[offset++] = (byte)(value >> 40);
            checksum ^= span[offset++] = (byte)(value >> 32);
            checksum ^= span[offset++] = (byte)(value >> 24);
            checksum ^= span[offset++] = (byte)(value >> 16);
            checksum ^= span[offset++] = (byte)(value >> 8);
            checksum ^= span[offset++] = (byte)(value >> 0);
            return default;
        }

        //public static void Write(DeviceType value, ref Span<byte> span, ref int offset, ref byte checksum) =>
        //    Write((byte)value, ref span, ref offset, ref checksum);

        //public static void Write(Channel value, ref Span<byte> span, ref int offset, ref byte checksum) =>
        //    Write((byte)value, ref span, ref offset, ref checksum);

        //public static void Write(ChannelMask value, ref Span<byte> span, ref int offset, ref byte checksum) =>
        //    Write((uint)value, ref span, ref offset, ref checksum);

        //public static void Write(Nwk.GroupAddress value, ref Span<byte> span, ref int offset, ref byte checksum) =>
        //    Write((ushort)value, ref span, ref offset, ref checksum);

        //public static void Write(Nwk.Address value, ref Span<byte> span, ref int offset, ref byte checksum) =>
        //    Write((ushort)value, ref span, ref offset, ref checksum);

        //public static void Write(Mac.Address value, ref Span<byte> span, ref int offset, ref byte checksum) =>
        //    Write((ulong)value, ref span, ref offset, ref checksum);

        //public static void Write(Mac.Endpoint value, ref Span<byte> span, ref int offset, ref byte checksum) =>
        //    Write((byte)value, ref span, ref offset, ref checksum);

        public static bool ReadBoolean(ref ReadOnlySpan<byte> span, ref int offset) =>
            span[offset++] == 1;

        public static byte ReadByte(ref ReadOnlySpan<byte> span, ref int offset) =>
            span[offset++];

        public static ushort ReadUInt16(ref ReadOnlySpan<byte> span, ref int offset) =>
            (ushort)((span[offset++] << 8) | span[offset++]);

        public static ushort ReadUInt16(ReadOnlyMemory<byte> buffer, int offset) =>
            (ushort)((buffer.Span[offset++] << 8) | buffer.Span[offset++]);

        public static uint ReadUInt32(ref ReadOnlySpan<byte> span, ref int offset) =>
            (uint)((span[offset++] << 24) | (span[offset++] << 16) | (span[offset++] << 8) | span[offset++]);

        public static ulong ReadUInt64(ref ReadOnlySpan<byte> span, ref int offset) =>
            ((ulong)ReadUInt32(ref span, ref offset) << 32) | (ulong)ReadUInt32(ref span, ref offset);

        //public static DeviceType ReadDeviceType(ref ReadOnlySpan<byte> span, ref int offset) =>
        //    (DeviceType)ReadByte(ref span, ref offset);

        //public static Channel ReadChannel(ref ReadOnlySpan<byte> span, ref int offset) =>
        //    (Channel)ReadByte(ref span, ref offset);

        //public static Nwk.GroupAddress ReadGrpAddr(ref ReadOnlySpan<byte> span, ref int offset) =>
        //    new Nwk.GroupAddress(ReadUInt16(ref span, ref offset));

        //public static Nwk.Address ReadNwkAddr(ref ReadOnlySpan<byte> span, ref int offset) =>
        //    new Nwk.Address(ReadUInt16(ref span, ref offset));

        //public static Mac.Address ReadExtAddr(ref ReadOnlySpan<byte> span, ref int offset) =>
        //    new Mac.Address(ReadUInt64(ref span, ref offset));

        //public static Mac.Endpoint ReadEndpoint(ref ReadOnlySpan<byte> span, ref int offset) =>
        //    new Mac.Endpoint(ReadByte(ref span, ref offset));

        //public static IReadOnlyList<DevicePayload> ReadDevices(ref ReadOnlySpan<byte> span, ref int offset)
        //{
        //    var devices = new List<DevicePayload>();
        //    while (offset < span.Length - 13)
        //        devices.Add(DevicePayload.Read(ref span, ref offset));
        //    return devices;
        //}

        //public static IReadOnlyList<NeighborPayload> ReadNeighbors(ref ReadOnlySpan<byte> span, ref int offset)
        //{
        //    var entriesCount = BigEndianBinaryPrimitives.ReadByte(ref span, ref offset);
        //    var startIndex = BigEndianBinaryPrimitives.ReadByte(ref span, ref offset);
        //    _ = startIndex; // not used!

        //    var neighbors = new List<NeighborPayload>(entriesCount);
        //    while (offset < span.Length - 24)
        //        neighbors.Add(NeighborPayload.Read(ref span, ref offset));
        //    return neighbors;
        //}
    }
}
