using System;

namespace Lsquared.SmartHome.Zigbee.Protocol.Raw
{
    public interface IPacketEncoder
    {
        ReadOnlyMemory<byte> Decode(ReadOnlyMemory<byte> raw);

        ReadOnlyMemory<byte> Encode(ReadOnlyMemory<byte> raw);
    }
}
