using System;
using System.Diagnostics.CodeAnalysis;

namespace Lsquared.SmartHome.Zigbee.MAC
{
    public readonly struct ExtPanID : IComparable<ExtPanID>, IEquatable<ExtPanID>, IFormattable
    {
        public static readonly ExtPanID None = 0UL;

        public ExtPanID(ulong value) => _value = value;

        public int CompareTo([AllowNull] ExtPanID other) =>
            _value.CompareTo(other._value);

        public bool Equals([AllowNull] ExtPanID other) =>
            Equals(this, other);

        public override bool Equals(object? obj) =>
            obj is ExtPanID other && Equals(this, other);

        public override int GetHashCode() =>
            _value.GetHashCode();

        public override string ToString() =>
            _value.ToString("X16");

        public string ToString(string? format, IFormatProvider? formatProvider) =>
            _value.ToString(format, formatProvider);

        public static explicit operator ulong(ExtPanID self) => self._value;

        public static implicit operator ExtPanID(ulong value) => new(value);

        public static bool Equals(ExtPanID lhs, ExtPanID rhs) => lhs._value.Equals(rhs._value);

        public static bool operator ==(ExtPanID lhs, ExtPanID rhs) => Equals(lhs, rhs);

        public static bool operator !=(ExtPanID lhs, ExtPanID rhs) => !Equals(lhs, rhs);

        private readonly ulong _value;
    }
}
