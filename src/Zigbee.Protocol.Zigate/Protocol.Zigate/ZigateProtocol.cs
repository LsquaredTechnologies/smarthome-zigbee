using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Lsquared.SmartHome.Buffers;
using Lsquared.SmartHome.Zigbee.Protocol.Raw;
using static Lsquared.Extensions.Functional;

namespace Lsquared.SmartHome.Zigbee.Protocol.Zigate
{
    public sealed partial class ZigateProtocol : IProtocol
    {
        public ushort ZigateVersion { get; private set; }

        public IPacketEncoder PacketEncoder { get; } = new ZigatePacketEncoder();

        public IPacketExtractor PacketExtractor { get; } = new ZigatePacketExtractor();

        public async ValueTask InitializeAsync(IZigbeeNetwork network)
        {
            var versionResponsePayload = await network.SendAndReceiveAsync<ZDO.Mgmt.GetVersionResponsePayload>(new ZDO.Mgmt.GetVersionRequestPayload());
            if (versionResponsePayload is not null)
                ZigateVersion = versionResponsePayload.SdkVersion;

            // TODO add more initialization...
        }

        public Func<ICommand, bool> ExpectResponseCode(ushort responseCode) =>
            (command) => (command.Header is ZigateCommandHeader h) && h.CommandCode == responseCode;

        public Request? CreateRequest(ICommandPayload payload) => payload switch
        {
            // ZDO: Device Discovery
            ZDO.GetDevicesRequestPayload => new ZDO.GetDevicesRequest(),
            ZDO.GetExtendedAddressRequestPayload p => new ZDO.GetExtendedAddressRequest(p),
            ZDO.GetNetworkAddressRequestPayload p => new ZDO.GetNetworkAddressRequest(p),
            // ZDO: Service Discovery
            ZDO.GetActiveEndpointsRequestPayload p => new ZDO.GetActiveEndpointsRequest(p),
            ZDO.GetNodeDescriptorRequestPayload p => new ZDO.GetNodeDescriptorRequest(p),
            ZDO.GetPowerDescriptorRequestPayload p => new ZDO.GetPowerDescriptorRequest(p),
            ZDO.GetMatchDescriptorRequestPayload p => new ZDO.GetMatchDescriptorRequest(p),
            ZDO.GetUserDescriptorRequestPayload p => new ZDO.GetUserDescriptorRequest(p),
            ZDO.GetSimpleDescriptorRequestPayload p => new ZDO.GetSimpleDescriptorRequest(p),
            ZDO.GetComplexDescriptorRequestPayload p => new ZDO.GetComplexDescriptorRequest(p),
            // ZDO: Binding

            // ZDO: Management
            ZDO.Mgmt.GetVersionRequestPayload => new ZDO.Mgmt.GetVersionRequest(),
            ZDO.Mgmt.PermitJoinRequestPayload p => new ZDO.Mgmt.PermitJoinRequest(p),
            ZDO.Mgmt.LeaveRequestPayload p => new ZDO.Mgmt.LeaveRequest(p),
            ZDO.Mgmt.NetworkUpdateRequestPayload p => new ZDO.Mgmt.NetworkUpdateRequest(p),
            ZDO.Mgmt.GetNeighborTableRequestPayload p => new ZDO.Mgmt.GetNeighborTableRequest(p),
            ZDO.Mgmt.GetRoutingTableRequestPayload => null, // not implemented?
            // ZCL General
            ZCL.ICommandPayload => throw new NotSupportedException("ZCL command payload must be encapsulated in ZCL command!"),
            // ZCL Attributes
            ZCL.Command<ZCL.ReadAttributesRequestPayload> p => new ZCL.ReadAttributeRequest(p),
            // 0x0000: Basic

            // 0x0003: Identify

            // 0x0004: Groups
            ZCL.Command<ZCL.Clusters.Groups.AddGroupRequestPayload> p => new ZCL.Clusters.Groups.AddGroupRequest(p),
            ZCL.Command<ZCL.Clusters.Groups.AddGroupIfIdentifyRequestPayload> p => new ZCL.Clusters.Groups.AddGroupIfIdentifyRequest(p),
            ZCL.Command<ZCL.Clusters.Groups.GetGroupMembershipRequestPayload> p => new ZCL.Clusters.Groups.GetGroupMembershipRequest(p),
            // 0x0005: Scenes

            // 0x0006: OnOff
            ZCL.Command<ZCL.Clusters.OnOff.OnOffRequestPayload> p => new ZCL.Clusters.OnOff.OnOffRequest(p),
            ZCL.Command<ZCL.Clusters.OnOff.OnOffWithEffectRequestPayload> p => new ZCL.Clusters.OnOff.OffWithEffectRequest(p),
            ZCL.Command<ZCL.Clusters.OnOff.OnOffWithTimedOffRequestPayload> p => new ZCL.Clusters.OnOff.OnWithTimedOffRequest(p),
            // 0x0008: Level
            ZCL.Command<ZCL.Clusters.Level.MoveLevelRequestPayload> p => new ZCL.Clusters.Level.MoveLevelRequest(p),
            ZCL.Command<ZCL.Clusters.Level.MoveToLevelRequestTimedPayload> p => new ZCL.Clusters.Level.MoveToLevelRequest(p),
            // 0x0101: DoorLock

            // 0x0102: WindowCovering

            // 0x0300: ColorControl
            ZCL.Command<ZCL.Clusters.Color.MoveToColorTemperatureRequestPayload> p => new ZCL.Clusters.Color.MoveToColorTemperatureRequest(p),
            // Error...
            _ => throw new NotSupportedException($"Cannot create request from payload: {payload.GetType()}")
        };

        #region Read methods

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

        #endregion

        #region Write methods

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

        #endregion

        #region Read

        private static ZigateCommandHeader ReadHeader(ref ReadOnlySpan<byte> span, ref int offset) =>
            new ZigateCommandHeader(
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset),
                BigEndianBinary.ReadByte(ref span, ref offset));

        private static ICommandPayload ReadPayload(ushort command, ref ReadOnlySpan<byte> span, ref int offset) => command switch
        {
            0x004D => ReadDeviceAnnounceIndicationPayload(ref span, ref offset),
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
            0x8040 => ReadGetNetworkAddressResponsePayload(ref span, ref offset),
            0x8041 => ReadGetExtendedAddressResponsePayload(ref span, ref offset),
            0x8042 => ReadGetNodeDescriptorResponsePayload(ref span, ref offset),
            0x8043 => ReadGetSimpleDescriptorResponsePayload(ref span, ref offset),
            0x8044 => ReadGetPowerDescriptorResponsePayload(ref span, ref offset),
            0x8045 => ReadGetActiveEndpointsResponsePayload(ref span, ref offset),
            0x8046 => ReadGetMatchDescriptorResponsePayload(ref span, ref offset),
            0x8047 => ReadLeaveResponsePayload(ref span, ref offset),
            //0x8048 => ReadLeaveIndicationPayload(ref span, ref offset),
            0x804A => ReadNetworkUpdateResponsePayload(ref span, ref offset),
            0x804E => ReadGetNeighborTableResponsePayload(ref span, ref offset),
            0x8060 => ReadAddGroupResponsePayload(ref span, ref offset),
            0x8062 => ReadGetGroupMembershipResponsePayload(ref span, ref offset),
            0x8100 => ReadReadSingleAttributeResponsePayload(ref span, ref offset),
            0x8102 => ReadReportSingleAttributeResponsePayload(ref span, ref offset),
            _ => ICommandPayload.None // Do not thrown; instead, returns an empty instance
        };

        private static Array<T> ReadArray<T>(ref ReadOnlySpan<byte> span, ref int offset, Reader<T> reader)
        {
            var count = BigEndianBinary.ReadByte(ref span, ref offset);
            var items = new List<T>(count);
            while (count-- > 0)
                items.Add(reader(ref span, ref offset));
            return new Array<T>(items);
        }

        #endregion

        #region Write

        private static Unit Write(object payload, ref Span<byte> span, ref int offset, ref byte checksum) => payload switch
        {
            Enum p => Write(p, ref span, ref offset, ref checksum),
            byte[] p => Write(p, ref span, ref offset, ref checksum),
            IArrayValue p => Write(p, ref span, ref offset, ref checksum),
            IValue p => Write(p, ref span, ref offset, ref checksum),
            ZCL.ICommand p => Write(p, ref span, ref offset, ref checksum),
            ICommandPayload p => Write(p, ref span, ref offset, ref checksum),
            _ => throw new NotSupportedException($"Not supported payload: \"{payload.GetType().Name}\"")
        };

        private static Unit Write(byte[] bytes, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            foreach (var b in bytes)
                BigEndianBinary.Write(b, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write<T>(T payload, ref Span<byte> span, ref int offset, ref byte checksum) where T : Enum => payload switch
        {
            NWK.Channel p => BigEndianBinary.Write((byte)p, ref span, ref offset, ref checksum),
            APP.AddressMode p => BigEndianBinary.Write((byte)p, ref span, ref offset, ref checksum),
            _ => throw new NotSupportedException($"Not supported payload: \"{payload.GetType().Name}\"")
        };

        private static Unit Write([NotNull] IArrayValue payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((byte)payload.Count, ref span, ref offset, ref checksum);
            foreach (var item in payload)
                if (item is not null)
                    Write(item, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write<T>([NotNull] Array<T> payload, ref Span<byte> span, ref int offset, ref byte checksum) where T : ICommandPayload
        {
            BigEndianBinary.Write((byte)payload.Count, ref span, ref offset, ref checksum);
            foreach (var item in payload)
                if (item is not null)
                    Write(item, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(IValue payload, ref Span<byte> span, ref int offset, ref byte checksum) => payload switch
        {
            // MAC layer
            MAC.Address p => BigEndianBinary.Write((ulong)p, ref span, ref offset, ref checksum),
            // NWK layer
            NWK.Address p => BigEndianBinary.Write((ushort)p, ref span, ref offset, ref checksum),
            NWK.GroupAddress p => BigEndianBinary.Write((ushort)p, ref span, ref offset, ref checksum),
            NWK.ChannelMask p => BigEndianBinary.Write((uint)p, ref span, ref offset, ref checksum),
            // APP layer
            APP.Address p => Write(p, ref span, ref offset, ref checksum),
            APP.Endpoint p => BigEndianBinary.Write((byte)p, ref span, ref offset, ref checksum),
            // ZDO structures
            ZDO.ComplexDescriptor p => Write(p, ref span, ref offset, ref checksum),
            ZDO.NeighborTableEntry p => Write(p, ref span, ref offset, ref checksum),
            ZDO.NodeDescriptor p => Write(p, ref span, ref offset, ref checksum),
            ZDO.RoutingTableEntry p => Write(p, ref span, ref offset, ref checksum),
            ZDO.SimpleDescriptor p => Write(p, ref span, ref offset, ref checksum),
            // Error...
            _ => throw new NotSupportedException($"Not supported payload: \"{payload.GetType().Name}\"")
        };

        private static Unit Write(ICommandPayload payload, ref Span<byte> span, ref int offset, ref byte checksum) => payload switch
        {
            NoCommandPayload => default,
            // Zigate-specific
            Zigbee.Zigate.GetTimeResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            Zigbee.Zigate.SetLedRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            Zigbee.Zigate.SetTimeRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            // ZDO: Device Discovery
            ZDO.DeviceAnnounceIndicationPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetExtendedAddressRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetExtendedAddressResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetNetworkAddressRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetNetworkAddressResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            // ZDO: Service Discovery
            ZDO.GetActiveEndpointsRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetActiveEndpointsResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetNodeDescriptorRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetNodeDescriptorResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetPowerDescriptorRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetPowerDescriptorResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetUserDescriptorRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetMatchDescriptorResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetUserDescriptorResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.SetUserDescriptorRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetSimpleDescriptorRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetSimpleDescriptorResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetComplexDescriptorRequestPayload p => Write(p, ref span, ref offset, ref checksum),
            ZDO.GetComplexDescriptorResponsePayload p => Write(p, ref span, ref offset, ref checksum),
            // ZDO: Management
            ZDO.Mgmt.GetVersionResponsePayload p => Write(p, ref span, ref offset, ref checksum),
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
            _ => throw new NotSupportedException($"Not supported payload: \"{payload.GetType().Name}\"")
        };

        #endregion

        #region Zigate-specific :: Read

        private static Zigbee.Zigate.StatusPayload ReadStatusPayload(ref ReadOnlySpan<byte> span, ref int offset) =>
             new Zigbee.Zigate.StatusPayload(
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadByte(ref span, ref offset),
                BigEndianBinary.ReadUInt16(ref span, ref offset));

        private static Zigbee.Zigate.GetTimeResponsePayload ReadGetTimeResponsePayload(ref ReadOnlySpan<byte> span, ref int offset) =>
            new Zigbee.Zigate.GetTimeResponsePayload(
                Year2k.AddSeconds(BigEndianBinary.ReadUInt32(ref span, ref offset)));

        #endregion

        #region Zigate-specific :: Write

        private static Unit Write(Zigbee.Zigate.GetTimeResponsePayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            var ts = (uint)(payload.DateTime.ToUniversalTime() - Year2k).TotalSeconds;
            BigEndianBinary.Write(ts, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(Zigbee.Zigate.SetLedRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.Status, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(Zigbee.Zigate.SetTimeRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            var ts = (uint)(payload.Time - Year2k).TotalSeconds;
            BigEndianBinary.Write(ts, ref span, ref offset, ref checksum);
            return default;
        }

        #endregion

        private delegate T Reader<T>(ref ReadOnlySpan<byte> span, ref int offset);

        private static readonly DateTimeOffset Year2k = new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero);
    }
}
