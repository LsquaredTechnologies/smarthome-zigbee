using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lsquared.SmartHome.Zigbee.Transports
{
    /// <summary>
    /// Contains method used to write data to transports.
    /// </summary>
    public interface ITransportWriter : IAsyncDisposable
    {
        /// <summary>
        /// Writes the specified packet to the underlying transport.
        /// </summary>
        /// <param name="packet">The packet to write.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task containing the operation result.</returns>
        ValueTask WriteAsync(ReadOnlyMemory<byte> packet, CancellationToken cancellationToken = default);
    }
}
