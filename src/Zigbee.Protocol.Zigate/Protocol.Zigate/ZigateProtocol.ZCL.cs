using System;
using System.Text;
using Lsquared.SmartHome.Buffers;
using Lsquared.SmartHome.Zigbee.ZCL;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Level;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Color;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.OnOff;
using static Lsquared.Extensions.Functional;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Identify;

namespace Lsquared.SmartHome.Zigbee.Protocol.Zigate
{
    /// <content>
    /// ZCL
    /// </content>
    partial class ZigateProtocol
    {
        #region General

        private static Unit Write(ZCL.ICommand payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            Write(payload.Address, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((byte)payload.SrcEndpoint, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((byte)payload.DstEndpoint, ref span, ref offset, ref checksum);
            return payload.Payload switch
            {
                // General
                ReadAttributesRequestPayload p => Write(p, ref span, ref offset, ref checksum),
                // Basic
                // Identify
                IdentifyRequestPayload p => BigEndianBinary.Write((ushort)(p.Duration.TotalSeconds), ref span, ref offset, ref checksum),
                // Groups
                AddGroupRequestPayload p => Write(p, ref span, ref offset, ref checksum),
                GetGroupMembershipRequestPayload p => Write(p, ref span, ref offset, ref checksum),
                // Scenes
                // OnOff
                OnOffRequestPayload p => BigEndianBinary.Write((byte)p.OnOff, ref span, ref offset, ref checksum),
                OnOffWithEffectRequestPayload p => Write(p, ref span, ref offset, ref checksum),
                OnOffWithTimedOffRequestPayload p => Write(p, ref span, ref offset, ref checksum),
                // Level
                MoveLevelRequestPayload p => Write(p, ref span, ref offset, ref checksum),
                MoveToLevelRequestTimedPayload p => Write(p, ref span, ref offset, ref checksum),
                // DoorLock
                // WindowCovering
                // Color
                MoveToColorTemperatureRequestPayload p => Write(p, ref span, ref offset, ref checksum),
                // Error...
                _ => throw new NotSupportedException($"Not supported payload: \"{payload.Payload.GetType().Name}\"")
            };
        }

        #endregion

        #region Attributes

        private static Unit Write(ReadAttributesRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.ClusterID, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((byte)0, ref span, ref offset, ref checksum); // direction: 0=server-to-client; 1=client-to-server
            BigEndianBinary.Write(false, ref span, ref offset, ref checksum); // manufacturer specific
            BigEndianBinary.Write((ushort)0, ref span, ref offset, ref checksum); // manufacturer ID
            BigEndianBinary.Write((byte)1, ref span, ref offset, ref checksum); // number of attributes to read
            BigEndianBinary.Write(payload.AttributeID, ref span, ref offset, ref checksum);
            return default;
        }

        private static ReadAttributesResponsePayload ReadReadSingleAttributeResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            var nwkAddr = BigEndianBinary.ReadUInt16(ref span, ref offset);
            var endpoint = BigEndianBinary.ReadByte(ref span, ref offset);
            var clusterID = BigEndianBinary.ReadUInt16(ref span, ref offset);

            return new ReadAttributesResponsePayload(
                nwkAddr,
                clusterID,
                endpoint,
                endpoint,
                new Array<AttributeResponsePayload>(new[]
                {
                    new AttributeResponsePayload(
                        BigEndianBinary.ReadUInt16(ref span, ref offset),
                        BigEndianBinary.ReadByte(ref span, ref offset),
                        new AttributeValue(
                            BigEndianBinary.ReadByte(ref span, ref offset),
                            BigEndianBinary.ReadUInt16(ref span, ref offset),
                            span[offset..^1].ToArray())) // ignore last RSSI/LQI value
                }));
        }

        private static ReportAttributesResponsePayload ReadReportSingleAttributeResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            var nwkAddr = BigEndianBinary.ReadUInt16(ref span, ref offset);
            var endpoint = BigEndianBinary.ReadByte(ref span, ref offset);
            var clusterID = BigEndianBinary.ReadUInt16(ref span, ref offset);

            return new ReportAttributesResponsePayload(
                nwkAddr,
                clusterID,
                endpoint,
                endpoint,
                new Array<ZCL.AttributeResponsePayload>(new[]
                {
                    new AttributeResponsePayload(
                        BigEndianBinary.ReadUInt16(ref span, ref offset),
                        BigEndianBinary.ReadByte(ref span, ref offset),
                        new AttributeValue(
                            BigEndianBinary.ReadByte(ref span, ref offset),
                            BigEndianBinary.ReadUInt16(ref span, ref offset),
                            span[offset..^1].ToArray())) // ignore last RSSI/LQI value
                }));
        }

        #endregion

        #region Basic Cluster

        #endregion

        #region Identify Cluster

        #endregion

        #region Groups Cluster

        private static Command<AddGroupResponsePayload> ReadAddGroupResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            var srcEndpoint = BigEndianBinary.ReadByte(ref span, ref offset);
            var clusterID = BigEndianBinary.ReadUInt16(ref span, ref offset);
            var status = BigEndianBinary.ReadByte(ref span, ref offset);
            NWK.GroupAddress grpAddr = BigEndianBinary.ReadUInt16(ref span, ref offset);
            NWK.Address srcNwkAddr = BigEndianBinary.ReadUInt16(ref span, ref offset);
            return new Command<AddGroupResponsePayload>(
                srcNwkAddr,
                srcEndpoint,
                1,
                new AddGroupResponsePayload(
                    srcEndpoint,
                    clusterID,
                    status,
                    grpAddr,
                    srcNwkAddr));
        }

        private static Command<GetGroupMembershipResponsePayload> ReadGetGroupMembershipResponsePayload(ref ReadOnlySpan<byte> span, ref int offset)
        {
            BigEndianBinary.ReadByte(ref span, ref offset); // Seq
            var srcEndpoint = BigEndianBinary.ReadByte(ref span, ref offset);
            var clusterID = BigEndianBinary.ReadUInt16(ref span, ref offset);
            //NWK.Address nwkAddr = BigEndianBinary.ReadUInt16(ref span, ref offset); // after 3.0d, before 3.0f
            var capacity = BigEndianBinary.ReadByte(ref span, ref offset);
            var groups = ReadArray<NWK.GroupAddress>(ref span, ref offset, (ref ReadOnlySpan<byte> a, ref int b) => BigEndianBinary.ReadUInt16(ref a, ref b));
            NWK.Address nwkAddr = BigEndianBinary.ReadUInt16(ref span, ref offset); // after 3.0f

            return new Command<GetGroupMembershipResponsePayload>(
                nwkAddr,
                srcEndpoint,
                1,
                new GetGroupMembershipResponsePayload(
                    capacity, // not in zigate
                    groups));
        }

        private static Unit Write(AddGroupRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((ushort)payload.GrpAddr, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((byte)payload.Name.Length, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((byte)payload.Name.Length, ref span, ref offset, ref checksum);
            Write(Encoding.ASCII.GetBytes(payload.Name), ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(GetGroupMembershipRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum) =>
            Write(payload.Addresses, ref span, ref offset, ref checksum);

        #endregion

        #region Scenes Cluster

        #endregion

        #region OnOff Cluster

        private static Unit Write(OnOffWithEffectRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write((byte)payload.Effect, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.EffectVariant, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(OnOffWithTimedOffRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.OnlyWhenOn, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.OnTimeInTenthOfSeconds, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.OffTimeInTenthOfSeconds, ref span, ref offset, ref checksum);
            return default;
        }

        #endregion

        #region Level Cluster

        private static Unit Write(MoveLevelRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.WithOnOff, ref span, ref offset, ref checksum);
            BigEndianBinary.Write((byte)payload.Direction, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.Rate, ref span, ref offset, ref checksum);
            return default;
        }

        private static Unit Write(MoveToLevelRequestTimedPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.WithOnOff, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.Level, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.TransitionTime, ref span, ref offset, ref checksum);
            return default;
        }

        #endregion

        #region DoorLock Cluster

        #endregion

        #region WindowCovering Cluster

        #endregion

        #region Color Cluster

        private static Unit Write(MoveToColorTemperatureRequestPayload payload, ref Span<byte> span, ref int offset, ref byte checksum)
        {
            BigEndianBinary.Write(payload.ColorTemperatureMired, ref span, ref offset, ref checksum);
            BigEndianBinary.Write(payload.TransitionTime, ref span, ref offset, ref checksum);
            return default;
        }

        #endregion

        #region Illuminance Cluster

        #endregion

        #region IlluminanceLevel Cluster

        #endregion

        #region Temperature Cluster

        #endregion

        #region Pressure Cluster

        #endregion

        #region RelativeHumidity Cluster

        #endregion
    }
}
