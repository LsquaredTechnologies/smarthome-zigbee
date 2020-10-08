using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Lsquared.SmartHome.Zigbee.Protocol.Raw;

namespace Lsquared.SmartHome.Zigbee.Transports.Internals
{
    /// <summary>
    /// Represents the default writer to write data to transports.
    /// </summary>
    public sealed class DefaultTransportWriter : ITransportWriter
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DefaultTransportWriter"/>.
        /// </summary>
        /// <param name="stream">The underlying stream of the transport.</param>
        /// <param name="encoder">The specific encoder to decode packets.</param>
        public DefaultTransportWriter(Stream stream, IPacketEncoder encoder) =>
            (_stream, _encoder) = (stream, encoder);

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            _stoppingCts.Cancel();
            return default;
        }

        /// <inheritdoc/>
        public ValueTask WriteAsync(ReadOnlyMemory<byte> packet, CancellationToken cancellationToken = default)
        {
            if (_stoppingCts.IsCancellationRequested) return default;
            if (cancellationToken.IsCancellationRequested) return default;

            return _stream.WriteAsync(_encoder.Encode(packet), cancellationToken);
        }

        private readonly CancellationTokenSource _stoppingCts = new();
        private readonly Stream _stream;
        private readonly IPacketEncoder _encoder;
    }
}
