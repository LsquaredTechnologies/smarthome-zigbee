using System;
using System.Diagnostics.CodeAnalysis;

namespace Lsquared.SmartHome.Zigbee.MAC
{
    public readonly struct PanID : IComparable<PanID>, IEquatable<PanID>, IFormattable
    {
        public static readonly PanID None = 0;

        public PanID(ushort value) => _value = value;

        public int CompareTo([AllowNull] PanID other) =>
            _value.CompareTo(other._value);

        public bool Equals([AllowNull] PanID other) =>
            Equals(this, other);

        public override bool Equals(object? obj) =>
            obj is PanID other && Equals(this, other);

        public override int GetHashCode() =>
            _value.GetHashCode();

        public override string ToString() =>
            _value.ToString("X4");

        public string ToString(string? format, IFormatProvider? formatProvider) =>
            _value.ToString(format, formatProvider);

        public static explicit operator ushort(PanID self) => self._value;

        public static implicit operator PanID(ushort value) => new(value);

        public static bool Equals(PanID lhs, PanID rhs) => lhs._value.Equals(rhs._value);

        public static bool operator ==(PanID lhs, PanID rhs) => lhs.Equals(rhs);

        public static bool operator !=(PanID lhs, PanID rhs) => !lhs.Equals(rhs);

        private readonly ushort _value;
    }
}
