using System;
using System.Runtime.InteropServices;

namespace Lsquared.SmartHome.Zigbee.APP
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct Address : IValue
    {
        [field: FieldOffset(0)]
        public AddressMode Mode { get; }

        [field: FieldOffset(1)]
        public NWK.Address NwkAddr { get; }

        [field: FieldOffset(1)]
        public NWK.GroupAddress GrpAddr { get; }

        [field: FieldOffset(1)]
        public MAC.Address ExtAddr { get; }

        public override string ToString() => Mode switch
        {
            AddressMode.Group => $"Mode: {Mode}, GrpAddr: {GrpAddr:X4}",
            AddressMode.Short => $"Mode: {Mode}, NwkAddr: {NwkAddr:X4}",
            AddressMode.IEEE => $"Mode: {Mode}, ExtAddr: {ExtAddr:X16}",
            _ => throw new NotSupportedException()
        };

        public Address(MAC.Address extAddr) => (Mode, NwkAddr, GrpAddr, ExtAddr) = (AddressMode.IEEE, 0, 0, extAddr);

        public Address(NWK.Address nwkAddr) => (Mode, ExtAddr, GrpAddr, NwkAddr) = (AddressMode.Short, 0L, 0, nwkAddr);

        public Address(NWK.GroupAddress grpAddr) => (Mode, ExtAddr, NwkAddr, GrpAddr) = (AddressMode.Group, 0L, 0, grpAddr);

        public static implicit operator Address(MAC.Address extAddr) => new Address(extAddr);

        public static implicit operator Address(NWK.Address nwkAddr) => new Address(nwkAddr);

        public static implicit operator Address(NWK.GroupAddress grpAddr) => new Address(grpAddr);
    }
}
