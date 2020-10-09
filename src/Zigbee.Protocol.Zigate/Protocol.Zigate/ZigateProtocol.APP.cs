using System;
using System.Collections.Generic;
using Lsquared.SmartHome.Buffers;
using static Lsquared.Extensions.Functional;

namespace Lsquared.SmartHome.Zigbee.Protocol.Zigate
{
    /// <content>
    /// APP Layer
    /// </content>
    partial class ZigateProtocol
    {
        #region Read

        private static Array<APP.Endpoint> ReadEndpoints(ref ReadOnlySpan<byte> span, ref int offset)
        {
            var count = BigEndianBinary.ReadByte(ref span, ref offset);
            var endpoints = new List<APP.Endpoint>(count);
            while (count-- > 0)
                endpoints.Add(BigEndianBinary.ReadByte(ref span, ref offset));
            return new Array<APP.Endpoint>(endpoints);
        }

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

        #endregion

        #region Write

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

        #endregion
    }
}
