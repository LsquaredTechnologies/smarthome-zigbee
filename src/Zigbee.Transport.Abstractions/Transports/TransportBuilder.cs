using System;

namespace Lsquared.SmartHome.Zigbee.Transports
{
    public sealed class TransportBuilder : ITransportBuilder
    {
        public ITransport Build() =>
            throw new InvalidOperationException("Cannot create transport without information");
    }
}
