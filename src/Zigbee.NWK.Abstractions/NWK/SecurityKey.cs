using System;

namespace Lsquared.SmartHome.Zigbee.NWK
{
    public readonly struct SecurityKey : IEquatable<SecurityKey>
    {
        public static readonly SecurityKey ZigbeeAlliance09 = SecurityKey.Parse("ZigBeeAlliance09");

        private SecurityKey(ulong hi, ulong lo) =>
            (_hi, _lo) = (hi, lo);

        public static SecurityKey Parse(string s)
        {
            var bytes = new byte[16];
            for (int i = 0; i < 16; bytes[i++] = (byte)s[i])
                ;

            var hihi = BitConverter.ToUInt64(bytes, 0);
            var hilo = BitConverter.ToUInt64(bytes, 4);
            var lohi = BitConverter.ToUInt64(bytes, 8);
            var lolo = BitConverter.ToUInt64(bytes, 12);

            var hi = hihi << 16 | hilo;
            var lo = lohi << 16 | lolo;

            return new SecurityKey(hi, lo);
        }

        public bool Equals(SecurityKey other) =>
            Equals(this, other);

        public override bool Equals(object? obj) =>
            obj is SecurityKey other && Equals(this, other);

        public override int GetHashCode() =>
            (_hi.GetHashCode() * 11) ^ _lo.GetHashCode();

        public override string ToString() =>
            _hi.ToString("X16") + _lo.ToString("X16");

        public static bool Equals(SecurityKey lhs, SecurityKey rhs) =>
            lhs._hi == rhs._hi && lhs._lo == rhs._lo;

        public static bool operator ==(SecurityKey lhs, SecurityKey rhs) =>
            Equals(lhs, rhs);

        public static bool operator !=(SecurityKey lhs, SecurityKey rhs) =>
            !Equals(lhs, rhs);

        public static implicit operator SecurityKey(ReadOnlySpan<byte> span) =>
            new SecurityKey(
                hi: (uint)(span[0] << 24 | span[1] << 16 | span[2] << 8 | span[3]),
                lo: (uint)(span[4] << 24 | span[5] << 16 | span[6] << 8 | span[7]));

        private readonly ulong _hi;
        private readonly ulong _lo;
    }
}
