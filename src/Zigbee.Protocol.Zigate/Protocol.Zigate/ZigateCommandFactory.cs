////using System;
////using Lsquared.SmartHome.Buffers;
////using Lsquared.SmartHome.Zigbee.Protocol.Commands;

////namespace Lsquared.SmartHome.Zigbee.Protocol.Zigate
////{
////    public sealed class ZigateCommandFactory : ICommandFactory
////    {
////        public ICommand? Create(ReadOnlySpan<byte> packetSpan)
////        {
////            var offset = 0;

////            var header = ReadHeader(ref packetSpan, ref offset);
////            if (header.Length + 5 != packetSpan.Length)
////                return null;

////            var payload = ReadPayload(header.CommandCode, ref packetSpan, ref offset);
////            return new ZigateCommand(header, payload);
////        }

////        private static ZigateCommandHeader ReadHeader(ref ReadOnlySpan<byte> span, ref int offset) =>
////             new ZigateCommandHeader(
////                BigEndianBinary.ReadUInt16(ref span, ref offset),
////                BigEndianBinary.ReadUInt16(ref span, ref offset),
////                BigEndianBinary.ReadByte(ref span, ref offset));

////        private static ICommandPayload ReadPayload(ushort command, ref ReadOnlySpan<byte> span, ref int offset) => command switch
////        {
////            //0x004D => DeviceAnnounceIndicationPayload.Read(ref span, ref offset),
////            //0x8000 => StatusPayload.Read(ref span, ref offset),
////            //0x8002 => DataIndicationPayload.Read(ref span, ref offset),
////            //0x8009 => GetNetworkStateResponsePayload.Read(ref span, ref offset),
////            //0x8010 => GetVersionResponsePayload.Read(ref span, ref offset),
////            //0x8011 => AckDataPayload.Read(ref span, ref offset),
////            //0x8014 => GetPermitJoinStatusPayload.Read(ref span, ref offset),
////            //0x8015 => GetDevicesResponsePayload.Read(ref span, ref offset),
////            //0x8017 => GetTimeResponsePayload.Read(ref span, ref offset),
////            //0x8024 => NetworkStartedResponsePayload.Read(ref span, ref offset),
////            ////0x8035 => PdmEventPayload.Read(ref span, ref offset),
////            ////0x804E => GetNeighborsResponsePayload.Read(ref span, ref offset),

////            //0x8102 => ReportAttributePayload.Read(ref span, ref offset),

////            _ => ICommandPayload.None
////        };
////    }
////}
