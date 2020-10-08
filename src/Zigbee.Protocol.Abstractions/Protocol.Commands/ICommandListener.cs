using System;

namespace Lsquared.SmartHome.Zigbee.Protocol.Commands
{
    public interface ICommandListener
    {
        void OnNext(ReadOnlyMemory<byte> raw);

        void OnNext(ICommand command);
    }
}
