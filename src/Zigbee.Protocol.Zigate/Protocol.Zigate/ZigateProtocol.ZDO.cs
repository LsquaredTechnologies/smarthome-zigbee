using System;
using System.Collections.Generic;
using Lsquared.SmartHome.Buffers;
using Lsquared.SmartHome.Zigbee.ZDO;
using static Lsquared.Extensions.Functional;

namespace Lsquared.SmartHome.Zigbee.Protocol.Zigate
{
    /// <content>
    /// ZDO
    /// </content>
    partial class ZigateProtocol
    {
        #region Device Discovery :: Read

        private static DeviceAnnounceIndicationPayload ReadDeviceAnnounceIndicationPayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new DeviceAnnounceIndicationPayload(
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                BigEndianBinary.ReadUInt64(ref span, ref offset),
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadBoolean(ref span, ref offset));

        private static GetDevicesResponsePayload ReadGetDevicesResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new GetDevicesResponsePayload(
                ReadDevices(ref span, ref offset));

        // TODO use ReadArray<T>?
        private static Array<DeviceResponsePayload> ReadDevices(ref ReadOnlySpan<byte> span, ref int offset)
        {
            var devices = new List<DeviceResponsePayload>();
            while (offset < span.Length - 13)
                devices.Add(ReadDeviceResponsePayload(ref span, ref offset));
            return new Array<DeviceResponsePayload>(devices);
        }

        private static DeviceResponsePayload ReadDeviceResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new DeviceResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                BigEndianBinary.ReadUInt64(ref span, ref offset),
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadByte(ref span, ref offset));

        private static GetNetworkAddressResponsePayload ReadGetNetworkAddressResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            return new GetNetworkAddressResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadUInt64(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadByte(ref span, ref offset));
            // TODO add device list...
        }

        private static GetExtendedAddressResponsePayload ReadGetExtendedAddressResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            return new GetExtendedAddressResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadUInt64(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadByte(ref span, ref offset));
            // TODO add device list..
        }

        #endregion

        #region Device Discovery :: Write

        private static Unit Write(DeviceAnnounceIndicationPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ulong)payload.ExtAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.MacCapabilities, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.Rejoin, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetNetworkAddressRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)NWK.Address.All, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ulong)payload.ExtAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.RequestType, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.StartIndex, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetNetworkAddressResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ulong)payload.ExtAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.Count, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.StartIndex, ref span, ref offset, ref checksum);
            ////foreach (var item in payload.NwkAddresses)
            ////    Write(item, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetExtendedAddressRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            var targetNwkAddr = payload.NwkAddr == NWK.Address.Coordinator
                    ? NWK.Address.Coordinator
                    : NWK.Address.All;

            BigEndianBinary.Write((ushort)targetNwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.RequestType, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.StartIndex, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetExtendedAddressResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ulong)payload.ExtAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.Count, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.StartIndex, ref span, ref offset, ref checksum);
            ////foreach (var item in payload.NwkAddresses)
            ////    Write(item, ref span, ref offset, ref checksum);
            return default;
        }

        #endregion

        #region Service Discovery :: Read

        private static GetActiveEndpointsResponsePayload ReadGetActiveEndpointsResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            return new GetActiveEndpointsResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                ReadEndpoints(ref span, ref offset));
        }

        private static GetNodeDescriptorResponsePayload ReadGetNodeDescriptorResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            return new GetNodeDescriptorResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                ReadNodeDescriptor(ref span, ref offset));
        }

        private static GetPowerDescriptorResponsePayload ReadGetPowerDescriptorResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            return new GetPowerDescriptorResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                0xFFFF, //BigEndianBinary.ReadUInt16(ref span, ref offset),
                new PowerDescriptor(BigEndianBinary.ReadUInt16(ref span, ref offset)));
        }

        private static GetMatchDescriptorResponsePayload ReadGetMatchDescriptorResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new GetMatchDescriptorResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                ReadEndpoints(ref span, ref offset));

        private static GetSimpleDescriptorResponsePayload ReadGetSimpleDescriptorResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            return new GetSimpleDescriptorResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                ReadSimpleDescriptor(ref span, ref offset));
        }

        #endregion

        #region Service Discovery :: Write

        private static Unit Write(GetActiveEndpointsRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetActiveEndpointsResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            Write(payload.ActiveEndpoints, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetNodeDescriptorRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetNodeDescriptorResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            Write(payload.NodeDescriptor, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetPowerDescriptorRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetPowerDescriptorResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.PowerDescriptor.BitsFlag, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetMatchDescriptorRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.ProfileID, ref span, ref offset, ref checksum);
            Write(payload.InputClusters, ref span, ref offset, ref checksum);
            Write(payload.OutputClusters, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetMatchDescriptorResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            Write(payload.MatchList, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetSimpleDescriptorRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((byte)payload.Endpoint, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetSimpleDescriptorResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            Write(payload.SimpleDescriptor, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetComplexDescriptorRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetComplexDescriptorResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            Write(payload.ComplexDescriptor, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetUserDescriptorRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetUserDescriptorResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((byte)payload.UserDescriptor.Tag.Length, ref span, ref offset, ref checksum);
            foreach (var b in payload.UserDescriptor.Tag.Span)
                span[offset++] = b;
            return default;
        }

        private static Unit Write(SetUserDescriptorRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((byte)payload.UserDescriptor.Length, ref span, ref offset, ref checksum);
            foreach (var b in payload.UserDescriptor.Span)
                span[offset++] = b;
            return default;
        }

        #endregion

        #region Structures

        private static NodeDescriptor ReadNodeDescriptor(ref ReadOnlySpan<byte> span, ref int offset) =>
            new NodeDescriptor
            {
                ManufacturerCode = BigEndianBinary.ReadUInt16(ref span, ref offset),
                MaxRxSize = BigEndianBinary.ReadUInt16(ref span, ref offset),
                MaxTxSize = BigEndianBinary.ReadUInt16(ref span, ref offset),
                ServerMask = BigEndianBinary.ReadUInt16(ref span, ref offset),
                DescriptorCapabilities = (DescriptorCapabilities)BigEndianBinary.ReadByte(ref span, ref offset),
                MacCapabilities = (MacCapabilities)BigEndianBinary.ReadByte(ref span, ref offset),
                MaxBufferSize = BigEndianBinary.ReadByte(ref span, ref offset),
                BitsFlag = BigEndianBinary.ReadUInt16(ref span, ref offset),
            };

        private static SimpleDescriptor ReadSimpleDescriptor(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Length
            return new SimpleDescriptor
            {
                Endpoint = BigEndianBinary.ReadByte(ref span, ref offset),
                ProfileID = BigEndianBinary.ReadUInt16(ref span, ref offset),
                DeviceID = BigEndianBinary.ReadUInt16(ref span, ref offset),
                DeviceVersion = BigEndianBinary.ReadByte(ref span, ref offset),
                InputClusters = ReadArray<ushort>(ref span, ref offset, BigEndianBinary.ReadUInt16),
                OutputClusters = ReadArray<ushort>(ref span, ref offset, BigEndianBinary.ReadUInt16),
            };
        }

        private static NeighborTableEntry ReadNeighborTableEntry(ref ReadOnlySpan<byte> span, ref int offset)
        {
            _ = BigEndianBinary.ReadUInt16(ref span, ref offset);
            MAC.ExtPanID extPanID = BigEndianBinary.ReadUInt64(ref span, ref offset);
            MAC.Address extAddr = BigEndianBinary.ReadUInt64(ref span, ref offset);
            var depth = BigEndianBinary.ReadByte(ref span, ref offset);
            var linkQuality = BigEndianBinary.ReadByte(ref span, ref offset);
            var flags = BigEndianBinary.ReadByte(ref span, ref offset);

            return new NeighborTableEntry(
                extPanID,
                extAddr,
                flags,
                0,
                depth,
                linkQuality);
        }

        #endregion
    }
}
