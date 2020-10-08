using System;
using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee.Transports
{
    /// <summary>
    /// Contains method used to read data from transports.
    /// </summary>
    public interface ITransportReader : IAsyncDisposable
    {
        /// <summary>
        /// Reads all packets from the underlying transport.
        /// </summary>
        /// <returns>A stream of packets.</returns>
        IAsyncEnumerable<ReadOnlyMemory<byte>> ReadAsync();
    }
}
