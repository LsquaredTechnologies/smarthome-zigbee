using System;

namespace Lsquared.SmartHome.Zigbee.Protocol.Commands
{
    public interface ICommandListener
    {
        void OnNext(ICommand command);
    }
    public interface IPacketListener
    {
        void OnNext(ReadOnlyMemory<byte> raw);
    }
}
