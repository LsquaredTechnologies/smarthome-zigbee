using System;
using System.Diagnostics.CodeAnalysis;

namespace Lsquared.SmartHome.Zigbee.NWK
{
    public readonly struct GroupAddress : IValue, IFormattable, IComparable<GroupAddress>, IEquatable<GroupAddress>
    {
        public GroupAddress(ushort value) => _value = value;

        public int CompareTo([AllowNull] GroupAddress other) =>
            _value.CompareTo(other._value);

        public bool Equals([AllowNull] GroupAddress other) =>
            _value.Equals(other._value);

        public override bool Equals(object? obj) =>
            obj is GroupAddress other && _value.Equals(other._value);

        public override int GetHashCode() =>
            _value.GetHashCode();

        public override string ToString() =>
            _value.ToString("X4");

        public string ToString(string? format, IFormatProvider? formatProvider) =>
            _value.ToString(format, formatProvider);

        public static explicit operator ushort(GroupAddress self) => self._value;

        public static implicit operator GroupAddress(ushort value) => new(value);

        public static bool operator ==(GroupAddress lhs, GroupAddress rhs) => lhs.Equals(rhs);

        public static bool operator !=(GroupAddress lhs, GroupAddress rhs) => !lhs.Equals(rhs);

        private readonly ushort _value;
    }
}
