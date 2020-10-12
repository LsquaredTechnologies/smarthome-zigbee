using System;
using System.Diagnostics.CodeAnalysis;

namespace Lsquared.SmartHome.Zigbee.NWK
{
    public readonly struct Address : IFormattable, IComparable<Address>, IEquatable<Address>
    {
        // Reserved = 0xFFF8-0xFFFA
        public static readonly Address Coordinator = 0x0000;
        public static readonly Address AllLowPowerRouters = 0xFFFB;
        public static readonly Address AllRouters = 0xFFFC;
        public static readonly Address AllRxOn = 0xFFFD;
        public static readonly Address Invalid = 0xFFFE;
        public static readonly Address All = 0xFFFF;

        public Address(ushort value) => _value = value;

        public int CompareTo([AllowNull] Address other) =>
            _value.CompareTo(other._value);

        public bool Equals([AllowNull] Address other) =>
            Equals(this, other);

        public override bool Equals(object? obj) =>
            obj is Address other && Equals(this, other);

        public override int GetHashCode() =>
            _value.GetHashCode();

        public override string ToString() =>
            _value.ToString("X4");

        public string ToString(string? format, IFormatProvider? formatProvider) =>
            _value.ToString(format, formatProvider);

        public static explicit operator ushort(Address self) => self._value;

        public static implicit operator Address(ushort value) => new(value);

        public static bool Equals(Address lhs, Address rhs) => lhs._value.Equals(rhs._value);

        public static bool operator ==(Address lhs, Address rhs) => Equals(lhs, rhs);

        public static bool operator !=(Address lhs, Address rhs) => !Equals(lhs, rhs);

        private readonly ushort _value;
    }
}
