using System;
using System.Diagnostics.CodeAnalysis;

namespace Lsquared.SmartHome.Zigbee.MAC
{
    public readonly struct Address : IComparable<Address>, IEquatable<Address>, IFormattable
    {
        public static readonly Address None = 0UL;
        public static readonly Address Invalid = 0xFFFFFFFFFFFFFFFFUL;

        public Address(ulong value) => _value = value;

        public int CompareTo([AllowNull] Address other) =>
            _value.CompareTo(other._value);

        public bool Equals([AllowNull] Address other) =>
            Equals(this, other);

        public override bool Equals(object? obj) =>
            obj is Address other && Equals(this, other);

        public override int GetHashCode() =>
            _value.GetHashCode();

        public override string ToString() =>
            _value.ToString("X16");

        public string ToString(string? format, IFormatProvider? formatProvider) =>
            _value.ToString(format, formatProvider);

        public static explicit operator ulong(Address self) => self._value;

        public static implicit operator Address(ulong value) => new(value);

        public static bool Equals(Address lhs, Address rhs) => lhs._value.Equals(rhs._value);

        public static bool operator ==(Address lhs, Address rhs) => Equals(lhs, rhs);

        public static bool operator !=(Address lhs, Address rhs) => !Equals(lhs, rhs);

        private readonly ulong _value;
    }
}
