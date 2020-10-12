using System;
using System.Collections.Generic;
using Lsquared.SmartHome.Buffers;
using Lsquared.SmartHome.Zigbee.ZDO;
using Lsquared.SmartHome.Zigbee.ZDO.Mgmt;
using static Lsquared.Extensions.Functional;

namespace Lsquared.SmartHome.Zigbee.Protocol.Zigate
{
    /// <content>
    /// ZDO Management
    /// </content>
    partial class ZigateProtocol
    {
        #region Management :: Read

        private static GetVersionResponsePayload ReadGetVersionResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new GetVersionResponsePayload(
               LittleEndianBinary.ReadUInt16(ref span, ref offset), // Bug in Zigate?
               BigEndianBinary.ReadUInt16(ref span, ref offset));

        private static GetPermitJoinStatusPayload ReadGetPermitJoinStatusPayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new GetPermitJoinStatusPayload(
                BigEndianBinary.ReadBoolean(ref span, ref offset));

        private static GetNetworkStateResponsePayload ReadGetNetworkStateResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new GetNetworkStateResponsePayload(
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                BigEndianBinary.ReadUInt64(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                BigEndianBinary.ReadUInt64(ref span, ref offset),
                (NWK.Channel)BigEndianBinary.ReadByte(ref span, ref offset));

        private static NetworkStartedResponsePayload ReadNetworkStartedResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new NetworkStartedResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                BigEndianBinary.ReadUInt64(ref span, ref offset),
                (NWK.Channel)BigEndianBinary.ReadByte(ref span, ref offset));

        private static NetworkUpdateResponsePayload ReadNetworkUpdateResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new NetworkUpdateResponsePayload(BigEndianBinary.ReadByte(ref span, ref offset));

        private static GetNeighborTableResponsePayload ReadGetNeighborTableResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            var status = BigEndianBinary.ReadByte(ref span, ref offset);
            var capacity = BigEndianBinary.ReadByte(ref span, ref offset);
            var count = BigEndianBinary.ReadByte(ref span, ref offset);
            var startIndex = BigEndianBinary.ReadByte(ref span, ref offset);

            var items = new List<NeighborTableEntry>(count);
            while (count-- > 0)
                items.Add(ReadNeighborTableEntry(ref span, ref offset));

            NWK.Address srcNwkAddr = BigEndianBinary.ReadUInt16(ref span, ref offset);

            return new GetNeighborTableResponsePayload(
                status,
                srcNwkAddr,
                capacity,
                startIndex,
                new Array<NeighborTableEntry>(items));
        }

        private static LeaveResponsePayload ReadLeaveResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new LeaveResponsePayload(BigEndianBinary.ReadByte(ref span, ref offset));

        //private static LeaveIndicationPayload ReadLeaveIndicationPayload(ref  ReadOnlySpan<byte> span, ref int offset) { }

        #endregion

        #region Management :: Write

        private static Unit Write(GetVersionResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            LittleEndianBinary.Write(payload.ZigbeeVersion, ref span, ref offset, ref checksum); // Bug in Zigate?
            BigEndianBinary.Write(payload.SdkVersion, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(PermitJoinRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.DstNwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.Duration, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.TrustCenterSignificance, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(PermitJoinResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(NetworkUpdateResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetNeighborTableRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.DstNwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.StartIndex, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetNeighborTableResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.Capacity, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.StartIndex, ref span, ref offset, ref checksum);
            Write(payload.NeighborTableEntries, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(LeaveRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ulong)payload.DstExtAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.Option, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(LeaveResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            return default;
        }

        #endregion
    }
}
