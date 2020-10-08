using System;

namespace Lsquared.SmartHome.Zigbee
{
    public abstract record Response : ICommand
    {
        public ICommandHeader Header { get; }

        public ICommandPayload Payload { get; }

        protected Response(ICommandHeader header, ICommandPayload payload) =>
            (Header, Payload) = (header, payload);

        public int WriteTo(ref Span<byte> span, int offset)
        {
            throw new NotImplementedException();
        }
    }
}
