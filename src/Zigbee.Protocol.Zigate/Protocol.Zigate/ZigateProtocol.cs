using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Lsquared.SmartHome.Buffers;
using Lsquared.SmartHome.Zigbee.Protocol.Raw;
using static Lsquared.Extensions.Functional;

namespace Lsquared.SmartHome.Zigbee.Protocol.Zigate
{
    public sealed class ZigateProtocol : IProtocol
    {
        public IPacketEncoder PacketEncoder { get; } = new ZigatePacketEncoder();

        public IPacketExtractor PacketExtractor { get; } = new ZigatePacketExtractor();

        public Request? CreateRequest(ICommandPayload payload) => payload switch
        {
            GetVersionRequestPayload => new GetVersionRequest(),

            ZDO.GetDevicesRequestPayload => new ZDO.GetDevicesRequest(),
            ZDO.GetActiveEndpointsRequestPayload p => new ZDO.GetActiveEndpointsRequest(p),
            ZDO.GetComplexDescriptorRequestPayload p => new ZDO.GetComplexDescriptorRequest(p),
            ZDO.GetExtendedAddressRequestPayload p => new ZDO.GetExtendedAddressRequest(p),
            ZDO.GetMatchDescriptorRequestPayload p => new ZDO.GetMatchDescriptorRequest(p),
            ZDO.GetNetworkAddressRequestPayload p => new ZDO.GetNetworkAddressRequest(p),
            ZDO.GetNodeDescriptorRequestPayload p => new ZDO.GetNodeDescriptorRequest(p),
            ZDO.GetSimpleDescriptorRequestPayload p => new ZDO.GetSimpleDescriptorRequest(p),
            ZDO.GetUserDescriptorRequestPayload p => new ZDO.GetUserDescriptorRequest(p),
            ZDO.GetPowerDescriptorRequestPayload p => new ZDO.GetPowerDescriptorRequest(p),

            ZDO.Mgmt.GetNeighborTableRequestPayload p => new ZDO.Mgmt.GetNeighborTableRequest(p),
            ZDO.Mgmt.GetRoutingTableRequestPayload => null, // not supported?
            ZDO.Mgmt.LeaveRequestPayload p => new ZDO.Mgmt.LeaveRequest(p),
            ZDO.Mgmt.NetworkUpdateRequestPayload p => new ZDO.Mgmt.NetworkUpdateRequest(p),
            ZDO.Mgmt.PermitJoinRequestPayload p => new ZDO.Mgmt.PermitJoinRequest(p),

            ZCL.ReadAttributesRequestPayload p => new ZCL.ReadAttributeRequest(p),

            ZCL.ICommandPayload => throw new NotSupportedException("ZCL command payload must be encapsulated in ZCL command!"),

            ZCL.Command<ZCL.Clusters.Groups.GetGroupMembershipRequestPayload> p => new ZCL.GetGroupMembershipRequest(p),

            ZCL.ICommand p => p.Payload switch
            {
                ZCL.Clusters.Level.MoveLevelRequestPayload => new ZCL.MoveLevelRequest(p),
                ZCL.Clusters.Level.MoveToColorTemperatureRequestPayload => new ZCL.MoveToColorTemperatureRequest(p),
                ZCL.Clusters.Level.MoveToLevelRequestTimedPayload => new ZCL.MoveToLevelRequest(p),
                ZCL.Clusters.OnOff.OnOffRequestPayload => new ZCL.OnOffRequest(p),
                ZCL.Clusters.OnOff.OnOffWithEffectRequestPayload => new ZCL.OffWithEffectRequest(p),
                ZCL.Clusters.OnOff.OnOffWithTimedOffRequestPayload => new ZCL.OnWithTimedOffRequest(p),
                ZCL.Clusters.Groups.AddGroupRequestPayload => new ZCL.AddGroupRequest(p),
                ZCL.Clusters.Groups.AddGroupIfIdentifyRequestPayload => new ZCL.AddGroupIfIdentifyRequest(p),

                _ => throw new NotSupportedException($"Cannot create request from payload: {p.Payload.GetType()}")
            },

            _ => throw new NotSupportedException($"Cannot create request from payload: {payload.GetType()}")
        };

        public Func<ICommand, bool> ExpectResponseCode(ushort responseCode) =>
            (command) => (command.Header is ZigateCommandHeader h) && h.CommandCode == responseCode;

        public ICommand Read(ReadOnlyMemory<byte> memory)
        {
            var offset = 0;
            var span = memory.Span;

            var header = ReadHeader(ref span, ref offset);
            if (header.Length + 5 != span.Length)
                return new ZigateCommand(header, ICommandPayload.None);

            var payload = ReadPayload(header.CommandCode, ref span, ref offset);
            return new ZigateCommand(header, payload);
        }

        public int Write(Request request, ref Span<byte> span)
        {
            byte checksum = 0;
            var offset = 0;
            var zero = offset;

            ((ZigateCommandHeader)request.Header).WriteTo(ref span, ref offset, ref checksum);
            Write(request.Payload, ref span, ref offset, ref checksum);

            ushort length = (ushort)(offset - 5); // payload length ONLY
            checksum ^= span[zero + 2] = (byte)(length >> 8);
            checksum ^= span[zero + 3] = (byte)(length >> 0);
            span[zero + 4] = checksum;

            return offset;
        }

        public int Write(Response response, ref Span<byte> span) =>
            throw new NotImplementedException();

        private static ZigateCommandHeader ReadHeader(ref ReadOnlySpan<byte> span, ref int offset) =>
            new ZigateCommandHeader(
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                BigEndianBinary.ReadByte(ref span, ref offset));

        private static ICommandPayload ReadPayload(ushort command, ref ReadOnlySpan<byte> span, ref int offset) => command switch
        {
            0x004D => ReadDeviceAnnouncePayload(ref span, ref offset),
            0x8000 => ReadStatusPayload(ref span, ref offset),
            //0x8002 => ReadDataIndicationPayload(ref span, ref offset),
            0x8009 => ReadGetNetworkStateResponsePayload(ref span, ref offset),
            0x8010 => ReadGetVersionResponsePayload(ref span, ref offset),
            //0x8011 => ReadAckDataPayload(ref span, ref offset),
            0x8014 => ReadGetPermitJoinStatusPayload(ref span, ref offset),
            0x8015 => ReadGetDevicesResponsePayload(ref span, ref offset),
            0x8017 => ReadGetTimeResponsePayload(ref span, ref offset),
            0x8024 => ReadNetworkStartedResponsePayload(ref span, ref offset),
            ////0x8035 => PdmEventPayload.Read(ref span, ref offset),
            0x8042 => ReadGetNodeDescriptorResponsePayload(ref span, ref offset),
            0x8043 => ReadGetSimpleDescriptorResponsePayload(ref span, ref offset),
            0x8044 => ReadGetPowerDescriptorResponsePayload(ref span, ref offset),
            0x8045 => ReadGetActiveEndpointsResponsePayload(ref span, ref offset),
            0x8046 => ReadGetMatchDescriptorResponsePayload(ref span, ref offset),
            0x8047 => ReadLeaveResponsePayload(ref span, ref offset),
            //0x8048 => ReadLeaveIndicationPayload(ref span, ref offset),
            0x804A => ReadNetworkUpdateResponsePayload(ref span, ref offset),
            0x804E => ReadGetNeighborTableResponsePayload(ref span, ref offset),

            0x8062 => ReadGetGroupMembershipResponsePayload(ref span, ref offset),

            0x8100 => ReadReadSingleAttributeResponsePayload(ref span, ref offset),
            0x8102 => ReadReportSingleAttributeResponsePayload(ref span, ref offset),

            _ => ICommandPayload.None
        };

        private static ZDO.DeviceAnnouncePayload ReadDeviceAnnouncePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new ZDO.DeviceAnnouncePayload(
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                BigEndianBinary.ReadUInt64(ref span, ref offset),
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadBoolean(ref span, ref offset));

        private static StatusPayload ReadStatusPayload(ref ReadOnlySpan<byte> span, ref int offset) =>
             new StatusPayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset));

        private static GetTimeResponsePayload ReadGetTimeResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new GetTimeResponsePayload(
                Year2k.AddSeconds(BigEndianBinary.ReadUInt32(ref span, ref offset)));

        private static GetNetworkStateResponsePayload ReadGetNetworkStateResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new GetNetworkStateResponsePayload(
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                BigEndianBinary.ReadUInt64(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                BigEndianBinary.ReadUInt64(ref span, ref offset),
                (NWK.Channel)BigEndianBinary.ReadByte(ref span, ref offset));

        private static GetVersionResponsePayload ReadGetVersionResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
             new GetVersionResponsePayload(
                LittleEndianBinary.ReadUInt16(ref span, ref offset), // Bug in Zigate?
                BigEndianBinary.ReadUInt16(ref span, ref offset));

        private static GetPermitJoinStatusPayload ReadGetPermitJoinStatusPayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new GetPermitJoinStatusPayload(
                BigEndianBinary.ReadBoolean(ref span, ref offset));

        private static ZDO.GetDevicesResponsePayload ReadGetDevicesResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new ZDO.GetDevicesResponsePayload(
                ReadDevices(ref span, ref offset));

        private static Array<ZDO.DeviceResponsePayload> ReadDevices(ref ReadOnlySpan<byte> span, ref int offset)
        {
            var devices = new List<ZDO.DeviceResponsePayload>();
            while (offset < span.Length - 13)
                devices.Add(ReadDeviceResponsePayload(ref span, ref offset));
            return new Array<ZDO.DeviceResponsePayload>(devices);
        }

        private static ZDO.DeviceResponsePayload ReadDeviceResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new ZDO.DeviceResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                BigEndianBinary.ReadUInt64(ref span, ref offset),
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadByte(ref span, ref offset));

        private static NetworkStartedResponsePayload ReadNetworkStartedResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new NetworkStartedResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                BigEndianBinary.ReadUInt64(ref span, ref offset),
                (NWK.Channel)BigEndianBinary.ReadByte(ref span, ref offset));

        private static ZDO.GetNodeDescriptorResponsePayload ReadGetNodeDescriptorResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            return new ZDO.GetNodeDescriptorResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                ReadNodeDescriptor(ref span, ref offset));
        }

        private static ZDO.NodeDescriptor ReadNodeDescriptor(ref ReadOnlySpan<byte> span, ref int offset) =>
            new ZDO.NodeDescriptor
            {
                ManufacturerCode = BigEndianBinary.ReadUInt16(ref span, ref offset),
                MaxRxSize = BigEndianBinary.ReadUInt16(ref span, ref offset),
                MaxTxSize = BigEndianBinary.ReadUInt16(ref span, ref offset),
                ServerMask = BigEndianBinary.ReadUInt16(ref span, ref offset),
                DescriptorCapabilities = BigEndianBinary.ReadByte(ref span, ref offset),
                MacCapabilities = BigEndianBinary.ReadByte(ref span, ref offset),
                MaxBufferSize = BigEndianBinary.ReadByte(ref span, ref offset),
                BitsFlag0 = BigEndianBinary.ReadByte(ref span, ref offset),
                BitsFlag1 = BigEndianBinary.ReadByte(ref span, ref offset),
            };

        private static ZDO.GetSimpleDescriptorResponsePayload ReadGetSimpleDescriptorResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            return new ZDO.GetSimpleDescriptorResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                ReadSimpleDescriptor(ref span, ref offset));
        }

        private static ZDO.SimpleDescriptor ReadSimpleDescriptor(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Length
            return new ZDO.SimpleDescriptor
            {
                Endpoint = BigEndianBinary.ReadByte(ref span, ref offset),
                ProfileID = BigEndianBinary.ReadUInt16(ref span, ref offset),
                DeviceID = BigEndianBinary.ReadUInt16(ref span, ref offset),
                DeviceVersion = BigEndianBinary.ReadByte(ref span, ref offset),
                InputClusters = ReadArray<ushort>(ref span, ref offset, BigEndianBinary.ReadUInt16),
                OutputClusters = ReadArray<ushort>(ref span, ref offset, BigEndianBinary.ReadUInt16),
            };
        }

        private static ZDO.GetPowerDescriptorResponsePayload ReadGetPowerDescriptorResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            return new ZDO.GetPowerDescriptorResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                0xFFFF, //BigEndianBinary.ReadUInt16(ref span, ref offset),
                new ZDO.PowerDescriptor(BigEndianBinary.ReadUInt16(ref span, ref offset)));
        }

        private static ZDO.GetActiveEndpointsResponsePayload ReadGetActiveEndpointsResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            return new ZDO.GetActiveEndpointsResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                ReadEndpoints(ref span, ref offset));
        }

        private static Array<APP.Endpoint> ReadEndpoints(ref ReadOnlySpan<byte> span, ref int offset)
        {
            var count = BigEndianBinary.ReadByte(ref span, ref offset);
            var endpoints = new List<APP.Endpoint>(count);
            while (count-- > 0)
                endpoints.Add(BigEndianBinary.ReadByte(ref span, ref offset));
            return new Array<APP.Endpoint>(endpoints);
        }

        private static ZDO.GetMatchDescriptorResponsePayload ReadGetMatchDescriptorResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new ZDO.GetMatchDescriptorResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                ReadEndpoints(ref span, ref offset));

        private static ZDO.Mgmt.LeaveResponsePayload ReadLeaveResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new ZDO.Mgmt.LeaveResponsePayload(BigEndianBinary.ReadByte(ref span, ref offset));

        //private static ZDO.Mgmt.LeaveIndicationPayload ReadLeaveIndicationPayload(ref  ReadOnlySpan<byte> span, ref int offset) { }

        private static ZDO.Mgmt.NetworkUpdateResponsePayload ReadNetworkUpdateResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new ZDO.Mgmt.NetworkUpdateResponsePayload(BigEndianBinary.ReadByte(ref span, ref offset));

        private static ZDO.Mgmt.GetNeighborTableResponsePayload ReadGetNeighborTableResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new ZDO.Mgmt.GetNeighborTableResponsePayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadByte(ref span, ref offset),
                ReadArray(ref span, ref offset, ReadNeighborTableEntry));

        private static APP.Address ReadAddress(ref ReadOnlySpan<byte> span, ref int offset)
        {
            var addressMode = (APP.AddressMode)BigEndianBinary.ReadByte(ref span, ref offset);
            return addressMode switch
            {
                APP.AddressMode.Group => new APP.Address((NWK.GroupAddress)BigEndianBinary.ReadUInt16(ref span, ref offset)),
                APP.AddressMode.Short => new APP.Address((NWK.Address)BigEndianBinary.ReadUInt16(ref span, ref offset)),
                APP.AddressMode.IEEE => new APP.Address((MAC.Address)BigEndianBinary.ReadUInt64(ref span, ref offset)),
                _ => throw new NotSupportedException($"Address mode is not supported: {addressMode}")
            };
        }
        private static ZCL.Command<ZCL.Clusters.Groups.GetGroupMembershipResponsePayload> ReadGetGroupMembershipResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            var srcEndpoint = BigEndianBinary.ReadByte(ref span, ref offset);
            var clusterID = BigEndianBinary.ReadUInt16(ref span, ref offset);
            //NWK.Address nwkAddr = BigEndianBinary.ReadUInt16(ref span, ref offset); // after 3.0d, before 3.0f
            var capacity = BigEndianBinary.ReadByte(ref span, ref offset);
            var groups = ReadArray<NWK.GroupAddress>(ref span, ref offset, (ref ReadOnlySpan<byte> a, ref int b) => BigEndianBinary.ReadUInt16(ref a, ref b));
            NWK.Address nwkAddr = BigEndianBinary.ReadUInt16(ref span, ref offset); // after 3.0f

            return new ZCL.Command<ZCL.Clusters.Groups.GetGroupMembershipResponsePayload>(
                nwkAddr,
                srcEndpoint,
                1,
                new ZCL.Clusters.Groups.GetGroupMembershipResponsePayload(
                    capacity, // not in zigate
                    groups));
        }

        private delegate T Reader<T>(ref ReadOnlySpan<byte> span, ref int offset);

        private static Array<T> ReadArray<T>(ref ReadOnlySpan<byte> span, ref int offset, Reader<T> reader)
        {
            var count = BigEndianBinary.ReadByte(ref span, ref offset);
            var items = new List<T>(count);
            while (count-- > 0)
                items.Add(reader(ref span, ref offset));
            return new Array<T>(items);
        }

        private static ZDO.NeighborTableEntry ReadNeighborTableEntry(ref ReadOnlySpan<byte> span, ref int offset)
        {
            _ = BigEndianBinary.ReadUInt16(ref span, ref offset);
            var extPanID = BigEndianBinary.ReadUInt64(ref span, ref offset);
            var extAddr = BigEndianBinary.ReadUInt64(ref span, ref offset);
            var depth = BigEndianBinary.ReadByte(ref span, ref offset);
            var linkQuality = BigEndianBinary.ReadByte(ref span, ref offset);
            var flags = BigEndianBinary.ReadByte(ref span, ref offset);

            return new ZDO.NeighborTableEntry(
                extPanID,
                extAddr,
                flags,
                0,
                depth,
                linkQuality);
        }

        private static ZCL.ReadAttributesResponsePayload ReadReadSingleAttributeResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            var nwkAddr = BigEndianBinary.ReadUInt16(ref span, ref offset);
            var endpoint = BigEndianBinary.ReadByte(ref span, ref offset);
            var clusterID = BigEndianBinary.ReadUInt16(ref span, ref offset);

            return new ZCL.ReadAttributesResponsePayload(
                nwkAddr,
                clusterID,
                endpoint,
                endpoint,
                new Array<ZCL.AttributeResponsePayload>(new[]
                {
                    new ZCL.AttributeResponsePayload(
                        BigEndianBinary.ReadUInt16(ref span, ref offset),
                        BigEndianBinary.ReadByte(ref span, ref offset),
                        new ZCL.AttributeValue(
                            BigEndianBinary.ReadByte(ref span, ref offset),
                            BigEndianBinary.ReadUInt16(ref span, ref offset),
                            span[offset..^1].ToArray())) // ignore last RSSI/LQI value
                }));
        }

        private static ZCL.ReportAttributesResponsePayload ReadReportSingleAttributeResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            var nwkAddr = BigEndianBinary.ReadUInt16(ref span, ref offset);
            var endpoint = BigEndianBinary.ReadByte(ref span, ref offset);
            var clusterID = BigEndianBinary.ReadUInt16(ref span, ref offset);

            return new ZCL.ReportAttributesResponsePayload(
                nwkAddr,
                clusterID,
                endpoint,
                endpoint,
                new Array<ZCL.AttributeResponsePayload>(new[]
                {
                    new ZCL.AttributeResponsePayload(
                        BigEndianBinary.ReadUInt16(ref span, ref offset),
                        BigEndianBinary.ReadByte(ref span, ref offset),
                        new ZCL.AttributeValue(
                            BigEndianBinary.ReadByte(ref span, ref offset),
                            BigEndianBinary.ReadUInt16(ref span, ref offset),
                            span[offset..^1].ToArray())) // ignore last RSSI/LQI value
                }));
        }

        //private static ICommandPayload AttachDebuggerIfAny(Type payloadType)
        //{
        //    if (Debugger.IsAttached)
        //        Debugger.Break();
        //    throw new NotSupportedException($"Not supported payload: \"{payloadType}\"");
        //}

        //private static Unit Write<TEnum>(T payload, ref Span<byte> span, ref int offset, ref byte checksum) where T : Enum => payload switch
        //{
        //    NWK.AddressMode p => BigEndianBinary.Write((byte)p, ref span, ref offset, ref checksum),
        //    NWK.Channel p => BigEndianBinary.Write((byte)p, ref span, ref offset, ref checksum),
        //    _ => throw new NotSupportedException($"Not supported payload: \"{payload.GetType().Name}\"")
        //};

        private static Unit Write(object payload, ref Span<byte> span, ref int offset, ref byte checksum) => payload switch
        {
            Enum p => Write(p, ref span, ref offset, ref checksum),
            IValue p => Write(p, ref span, ref offset, ref checksum),
            ICommandPayload p => Write(p, ref span, ref offset, ref checksum),
            _ => throw new NotSupportedException($"Not supported payload: \"{payload.GetType().Name}\"")
        };

        private static Unit Write<T>(T payload, ref Span<byte> span, ref int offset, ref byte checksum) where T : struct => payload switch
        {
            bool p => BigEndianBinary.Write(p, ref span, ref offset, ref checksum),
            byte p => BigEndianBinary.Write(p, ref span, ref offset, ref checksum),
            ushort p => BigEndianBinary.Write(p, ref span, ref offset, ref checksum),
            uint p => BigEndianBinary.Write(p, ref span, ref offset, ref checksum),
            ulong p => BigEndianBinary.Write(p, ref span, ref offset, ref checksum),

            NWK.Channel p => BigEndianBinary.Write((byte)p, ref span, ref offset, ref checksum),
            APP.AddressMode p => BigEndianBinary.Write((byte)p, ref span, ref offset, ref checksum),

            IValue p => Write(p, ref span, ref offset, ref checksum),

            _ => throw new NotSupportedException($"Not supported payload: \"{payload.GetType().Name}\"")
        };

        private static Unit Write(IValue payload, ref Span<byte> span, ref int offset, ref byte checksum) => payload switch
        {
            MAC.Address p => BigEndianBinary.Write((ulong)p, ref span, ref offset, ref checksum),

            NWK.Address p => BigEndianBinary.Write((ushort)p, ref span, ref offset, ref checksum),
            NWK.GroupAddress p => BigEndianBinary.Write((ushort)p, ref span, ref offset, ref checksum),
            NWK.ChannelMask p => BigEndianBinary.Write((uint)p, ref span, ref offset, ref checksum),

            APP.Endpoint p => BigEndianBinary.Write((byte)p, ref span, ref offset, ref checksum),

            ZDO.ComplexDescriptor p => Write(p, ref span, ref offset, ref checksum),
            ZDO.NeighborTableEntry p => Write(p, ref span, ref offset, ref checksum),
            ZDO.NodeDescriptor p => Write(p, ref span, ref offset, ref checksum),
            ZDO.RoutingTableEntry p => Write(p, ref span, ref offset, ref checksum),
            ZDO.SimpleDescriptor p => Write(p, ref span, ref offset, ref checksum),

            _ => throw new NotSupportedException($"Not supported payload: \"{payload.GetType().Name}\"")
        };

        private static Unit Write(ICommandPayload payload, ref Span<byte> span, ref int offset, ref byte checksum) => payload switch
        {
            NoCommandPayload => default,
            IArrayValue p => Write(p, ref span, ref offset, ref checksum),

            // Zigate
            GetTimeResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            GetVersionResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            SetLedRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            SetTimeRequestPayload p => Write(p, ref span, ref offset, ref checksum),

            // ZDO
            ZDO.DeviceAnnouncePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetActiveEndpointsRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetActiveEndpointsResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetComplexDescriptorRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetComplexDescriptorResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetExtendedAddressRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetExtendedAddressResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetMatchDescriptorRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetMatchDescriptorResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetNetworkAddressRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetNetworkAddressResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetNodeDescriptorRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetNodeDescriptorResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetPowerDescriptorRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetPowerDescriptorResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetSimpleDescriptorRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetSimpleDescriptorResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetUserDescriptorRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetUserDescriptorResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.SetUserDescriptorRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.Mgmt.GetNeighborTableRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.Mgmt.GetNeighborTableResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.Mgmt.GetRoutingTableRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.Mgmt.GetRoutingTableResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.Mgmt.LeaveRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.Mgmt.LeaveResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.Mgmt.NetworkUpdateResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.Mgmt.PermitJoinRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.Mgmt.PermitJoinResponsePayload p => Write(p, ref span, ref offset, ref checksum),

            // ZCL
            ZCL.ICommand p => Write(p, ref span, ref offset, ref checksum),

            ZCL.ReadAttributesRequestPayload p => Write(p, ref span, ref offset, ref checksum),

            _ => throw new NotSupportedException($"Not supported payload: \"{payload.GetType().Name}\"")
        };

        private static Unit Write(GetTimeResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            var ts = (uint)(payload.DateTime.ToUniversalTime() - Year2k).TotalSeconds;
            BigEndianBinary.Write(ts, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetVersionResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            LittleEndianBinary.Write(payload.ZigbeeVersion, ref span, ref offset, ref checksum); // Bug in Zigate?
            BigEndianBinary.Write(payload.SdkVersion, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(SetLedRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(SetTimeRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            var ts = (uint)(payload.Time - Year2k).TotalSeconds;
            BigEndianBinary.Write(ts, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write([NotNull] IArrayValue payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((byte)payload.Count, ref span, ref offset, ref checksum);
            foreach (var item in payload!)
                if (item is not null)
                    Write(item, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.DeviceAnnouncePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ulong)payload.ExtAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.MacCapabilities, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.Rejoin, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetActiveEndpointsRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetActiveEndpointsResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            Write(payload.ActiveEndpoints, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetComplexDescriptorRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetComplexDescriptorResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            Write(payload.ComplexDescriptor, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetExtendedAddressRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.RequestType, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.StartIndex, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetExtendedAddressResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ulong)payload.ExtAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.Count, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.StartIndex, ref span, ref offset, ref checksum);
            foreach (var item in payload.NwkAddresses)
                Write(item, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetMatchDescriptorRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.ProfileID, ref span, ref offset, ref checksum);
            Write(payload.InputClusters, ref span, ref offset, ref checksum);
            Write(payload.OutputClusters, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetMatchDescriptorResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            Write(payload.MatchList, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetNetworkAddressRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.ExtAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.RequestType, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.StartIndex, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetNetworkAddressResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ulong)payload.ExtAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.Count, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.StartIndex, ref span, ref offset, ref checksum);
            foreach (var item in payload.NwkAddresses)
                Write(item, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetNodeDescriptorRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetNodeDescriptorResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            Write(payload.NodeDescriptor, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetPowerDescriptorRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetPowerDescriptorResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.PowerDescriptor.BitsFlag, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetSimpleDescriptorRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((byte)payload.Endpoint, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetSimpleDescriptorResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            Write(payload.SimpleDescriptor, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetUserDescriptorRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.GetUserDescriptorResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((byte)payload.UserDescriptor.Length, ref span, ref offset, ref checksum);
            foreach (var b in payload.UserDescriptor.Span)
                span[offset++] = b;
            return default;
        }

        private static Unit Write(ZDO.SetUserDescriptorRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((byte)payload.UserDescriptor.Length, ref span, ref offset, ref checksum);
            foreach (var b in payload.UserDescriptor.Span)
                span[offset++] = b;
            return default;
        }

        private static Unit Write(ZDO.Mgmt.GetRoutingTableRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.StartIndex, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.Mgmt.GetRoutingTableResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.RoutingTableEntryTotalCount, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.StartIndex, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.RoutingTableEntryCount, ref span, ref offset, ref checksum);
            foreach (var item in payload.RoutingTableEntries)
                Write(item, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.Mgmt.GetNeighborTableRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.DstNwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.StartIndex, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.Mgmt.GetNeighborTableResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.NeighborTableEntryTotalCount, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.StartIndex, ref span, ref offset, ref checksum);
            Write(payload.NeighborTableEntries, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.Mgmt.LeaveRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ulong)payload.DstExtAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.Option, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.Mgmt.LeaveResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.Mgmt.NetworkUpdateResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.Mgmt.PermitJoinRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.DstNwkAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.Duration, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.TrustCenterSignificance, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZDO.Mgmt.PermitJoinResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(APP.Address address, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((byte)address.Mode, ref span, ref offset, ref checksum);
            return address.Mode switch
            {
                APP.AddressMode.Group => BigEndianBinary.Write((ushort)address.GrpAddr, ref span, ref offset, ref checksum),
                APP.AddressMode.Short => BigEndianBinary.Write((ushort)address.NwkAddr, ref span, ref offset, ref checksum),
                APP.AddressMode.IEEE => BigEndianBinary.Write((ulong)address.ExtAddr, ref span, ref offset, ref checksum),
                _ => throw new NotSupportedException()
            };
        }

        private static Unit Write(ZCL.ICommand payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            Write(payload.Address, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((byte)payload.SrcEndpoint, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((byte)payload.DstEndpoint, ref span, ref offset, ref checksum);
            return payload.Payload switch
            {
                bool p => BigEndianBinary.Write(p, ref span, ref offset, ref checksum),
                byte p => BigEndianBinary.Write(p, ref span, ref offset, ref checksum),
                ushort p => BigEndianBinary.Write(p, ref span, ref offset, ref checksum),
                uint p => BigEndianBinary.Write(p, ref span, ref offset, ref checksum),
                ulong p => BigEndianBinary.Write(p, ref span, ref offset, ref checksum),
                NoCommandPayload => default,
                IArrayValue p => Write(p, ref span, ref offset, ref checksum),
                ZCL.Clusters.OnOff.OnOffRequestPayload p => Write((byte)p.OnOff, ref span, ref offset, ref checksum),
                ZCL.Clusters.Groups.AddGroupRequestPayload p => Write(p.GrpAddr, ref span, ref offset, ref checksum),
                ZCL.Clusters.Groups.GetGroupMembershipRequestPayload p => Write(p, ref span, ref offset, ref checksum),
                ZCL.Clusters.Level.MoveLevelRequestPayload p => Write(p, ref span, ref offset, ref checksum),
                ZCL.Clusters.Level.MoveToLevelRequestTimedPayload p => Write(p, ref span, ref offset, ref checksum),
                ZCL.Clusters.Level.MoveToColorTemperatureRequestPayload p => Write(p, ref span, ref offset, ref checksum),
                ZCL.Clusters.OnOff.OnOffWithEffectRequestPayload p => Write(p, ref span, ref offset, ref checksum),
                ZCL.Clusters.OnOff.OnOffWithTimedOffRequestPayload p => Write(p, ref span, ref offset, ref checksum),


                _ => throw new NotSupportedException($"Not supported payload: \"{payload.Payload.GetType().Name}\"")
            };
        }

        //private static Unit Write(ZCL.Command payload, ref Span<byte> span, ref int offset, ref byte checksum)
        //{
        //    return default;
        //}

        //private static Unit Write<T>(ZCL.Command<T> payload, ref Span<byte> span, ref int offset, ref byte checksum)
        //{
        //    BigEndianBinary.Write((byte)0x02, ref span, ref offset, ref checksum);
        //    BigEndianBinary.Write((ushort)payload.NwkAddr, ref span, ref offset, ref checksum);
        //    BigEndianBinary.Write((byte)payload.SrcEndpoint, ref span, ref offset, ref checksum);
        //    BigEndianBinary.Write((byte)payload.DstEndpoint, ref span, ref offset, ref checksum);

        //    return payload.Payload switch
        //    {
        //        ZCL.Clusters.Groups.GroupPayload p => Write(p, ref span, ref offset, ref checksum),
        //        ZCL.Clusters.Level.MoveLevelPayload p => Write(p, ref span, ref offset, ref checksum),
        //        ZCL.Clusters.Level.MoveToLevelTimedPayload p => Write(p, ref span, ref offset, ref checksum),
        //        ZCL.Clusters.Level.MoveToColorTemperaturePayload p => Write(p, ref span, ref offset, ref checksum),
        //        ZCL.Clusters.OnOff.OnOffWithEffectPayload p => Write(p, ref span, ref offset, ref checksum),
        //        ZCL.Clusters.OnOff.OnOffWithTimedOffPayload p => Write(p, ref span, ref offset, ref checksum),
        //        _ => throw new NotSupportedException($"Not supported payload: \"{payload.Payload.GetType().Name}\"")
        //    };
        //}

        private static Unit Write(ZCL.Clusters.Groups.AddGroupRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.GrpAddr, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZCL.Clusters.Level.MoveLevelRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.WithOnOff, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((byte)payload.Direction, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.Rate, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write<T>(Array<T> array, ref Span<byte> span, ref int offset, ref byte checksum) where T : ICommandPayload
        {
            BigEndianBinary.Write((byte)array.Count, ref span, ref offset, ref checksum);
            foreach (var item in array)
                Write(item, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZCL.Clusters.Groups.GetGroupMembershipRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum) =>
            Write(payload.Addresses, ref span, ref offset, ref checksum);

        private static Unit Write(ZCL.Clusters.Level.MoveToLevelRequestTimedPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.WithOnOff, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.Level, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.TransitionTime, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZCL.Clusters.Level.MoveToColorTemperatureRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.ColorTemperatureMired, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.TransitionTime, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZCL.Clusters.OnOff.OnOffWithEffectRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((byte)payload.Effect, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.EffectVariant, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZCL.Clusters.OnOff.OnOffWithTimedOffRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.OnlyWhenOn, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.OnTimeInTenthOfSeconds, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.OffTimeInTenthOfSeconds, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(ZCL.ReadAttributesRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            Write(payload.Address, ref span, ref offset, ref checksum);
            Write(payload.SrcEndpoint, ref span, ref offset, ref checksum);
            Write(payload.DstEndpoint, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.ClusterID, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((byte)0, ref span, ref offset, ref checksum); // direction: 0=server-to-client; 1=client-to-server
            BigEndianBinary.Write(false, ref span, ref offset, ref checksum); // manufacturer specific
            BigEndianBinary.Write((ushort)0, ref span, ref offset, ref checksum); // manufacturer ID
            BigEndianBinary.Write((byte)1, ref span, ref offset, ref checksum); // number of attributes to read
            BigEndianBinary.Write(payload.AttributeID, ref span, ref offset, ref checksum);
            return default;
        }

        private static readonly DateTimeOffset Year2k = new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero);
    }
}
