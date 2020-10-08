namespace Lsquared.SmartHome.Zigbee
{
    public abstract record Request : ICommand
    {
        public ICommandHeader Header { get; init; }

        public ICommandPayload Payload { get; }

        public virtual ushort ExpectedResponseCode { get; }

        protected Request(ICommandHeader header, ICommandPayload payload) =>
            (Header, Payload) = (header, payload);

        ////public int WriteTo(ref Span<byte> span, int offset)
        ////{
        ////    throw new NotImplementedException();
        ////}
    }
}
