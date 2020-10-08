using System;

namespace Lsquared.SmartHome.Zigbee.NWK
{
    public readonly struct ChannelMask : IValue, IFormattable //, IEquatable<ChannelMask>
    {
        public static readonly ChannelMask Empty = default;

        private ChannelMask(uint value) => _value = value;

        public string ToString(string? format, IFormatProvider? formatProvider) =>
            _value.ToString(format, formatProvider);

        public static ChannelMask operator |(ChannelMask lhs, int rhs) =>
            new ChannelMask(lhs._value | (uint)(1 << rhs));

        public static ChannelMask operator |(ChannelMask lhs, Channel rhs) =>
            new ChannelMask(lhs._value | (uint)(1 << (int)rhs));

        public static ChannelMask operator |(ChannelMask lhs, ChannelMask rhs) =>
            new ChannelMask(lhs._value | rhs._value);

        public static explicit operator uint (ChannelMask self) =>
            self._value;

        public static explicit operator ChannelMask(int channel) =>
            new((uint)(1 << channel));

        public static explicit operator ChannelMask(uint channel) =>
            new((uint)(1 << (int)channel));

        public static explicit operator ChannelMask(Channel channel) =>
            new((uint)(1 << (int)channel));

        private readonly uint _value;
    }
}
