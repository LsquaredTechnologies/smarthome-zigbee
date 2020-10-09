using System;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed class UserDescriptor
    {
        public ReadOnlyMemory<byte> Tag { get; init; } = Empty;

        private static readonly ReadOnlyMemory<byte> Empty = new byte[0];
    }
}
