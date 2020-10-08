using System;

namespace Lsquared.SmartHome.Zigbee.Protocol.Raw
{
    public interface IPacketSubscriber
    {
        IDisposable Subscribe(IPacketListener listener);
    }
}
