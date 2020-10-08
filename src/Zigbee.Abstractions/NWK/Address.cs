using System;
using System.Diagnostics.CodeAnalysis;

namespace Lsquared.SmartHome.Zigbee.NWK
{
    public readonly struct Address : IValue, IFormattable, IComparable<Address>, IEquatable<Address>
    {
        public static readonly Address Coordinator = 0;
        public static readonly Address AllRouters = 0xFFFC;

        public Address(ushort value) => _value = value;

        public int CompareTo([AllowNull] Address other) =>
            _value.CompareTo(other._value);

        public bool Equals([AllowNull] Address other) =>
            _value.Equals(other._value);

        public override bool Equals(object? obj) =>
            obj is Address other && _value.Equals(other._value);

        public override int GetHashCode() =>
            _value.GetHashCode();

        public override string ToString() =>
            _value.ToString("X4");

        public string ToString(string? format, IFormatProvider? formatProvider) =>
            _value.ToString(format, formatProvider);

        public static explicit operator ushort(Address self) => self._value;

        public static implicit operator Address(ushort value) => new(value);

        public static bool operator ==(Address lhs, Address rhs) => lhs.Equals(rhs);

        public static bool operator !=(Address lhs, Address rhs) => !lhs.Equals(rhs);

        private readonly ushort _value;
    }
}
