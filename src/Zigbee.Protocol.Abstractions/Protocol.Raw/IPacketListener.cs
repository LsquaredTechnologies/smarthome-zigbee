using System;

namespace Lsquared.SmartHome.Zigbee.Protocol.Raw
{
    public interface IPacketListener
    {
        void OnNext(ReadOnlyMemory<byte> raw);
    }
}
