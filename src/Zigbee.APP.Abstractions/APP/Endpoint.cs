using System;
using System.Diagnostics.CodeAnalysis;

namespace Lsquared.SmartHome.Zigbee.APP
{
    public readonly struct Endpoint : IComparable<Endpoint>, IEquatable<Endpoint>, IFormattable
    {
        public Endpoint(byte value) => _value = value;

        public int CompareTo([AllowNull] Endpoint other) =>
            _value.CompareTo(other._value);

        public bool Equals([AllowNull] Endpoint other) =>
            _value.Equals(other._value);

        public override bool Equals(object? obj) =>
            obj is Endpoint other && _value.Equals(other._value);

        public override int GetHashCode() =>
            _value.GetHashCode();

        public override string ToString() =>
            _value.ToString("X2");

        public string ToString(string? format, IFormatProvider? formatProvider) =>
            _value.ToString(format, formatProvider);

        public static explicit operator byte(Endpoint self) => self._value;

        public static implicit operator Endpoint(byte value) => new(value);

        public static implicit operator Endpoint(int value) => new((byte)value);

        public static bool operator ==(Endpoint lhs, Endpoint rhs) => lhs.Equals(rhs);

        public static bool operator !=(Endpoint lhs, Endpoint rhs) => !lhs.Equals(rhs);

        private readonly byte _value;
    }
}
