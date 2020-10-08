using System;

namespace Lsquared.SmartHome.Zigbee.Protocol.Commands
{
    public interface ICommandSubscriber
    {
        IDisposable Subscribe(ICommandListener listener);
    }
    public interface IPacketSubscriber
    {
        IDisposable Subscribe(IPacketListener listener);
    }
}
