using System;
using System.Diagnostics.CodeAnalysis;

namespace Lsquared.SmartHome.Zigbee.MAC
{
    public readonly struct Address : IValue, IFormattable, IComparable<Address>, IEquatable<Address>
    {
        public Address(ulong value) => _value = value;

        public int CompareTo([AllowNull] Address other) =>
            _value.CompareTo(other._value);

        public bool Equals([AllowNull] Address other) =>
            _value.Equals(other._value);

        public override bool Equals(object? obj) =>
            obj is Address other && _value.Equals(other._value);

        public override int GetHashCode() =>
            _value.GetHashCode();

        public override string ToString() =>
            _value.ToString("X16");

        public string ToString(string? format, IFormatProvider? formatProvider) =>
            _value.ToString(format, formatProvider);

        public static explicit operator ulong(Address self) =>self._value;

        public static implicit operator Address(ulong value) => new(value);

        public static bool operator ==(Address lhs, Address rhs) => lhs.Equals(rhs);

        public static bool operator !=(Address lhs, Address rhs) => !lhs.Equals(rhs);

        private readonly ulong _value;
    }
}
