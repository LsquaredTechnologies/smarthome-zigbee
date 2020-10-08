using System;

namespace Lsquared.SmartHome.Zigbee.Protocol.Zigate
{
    public sealed record ZigateCommand(ZigateCommandHeader Header, ICommandPayload Payload) : ICommand
    {
        ICommandHeader ICommand.Header => Header;

        public int WriteTo(ref Span<byte> span, int offset)
        {
            throw new NotImplementedException();
        }
    }
}
