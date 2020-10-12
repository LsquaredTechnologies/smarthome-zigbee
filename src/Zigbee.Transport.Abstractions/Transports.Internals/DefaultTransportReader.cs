using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Lsquared.SmartHome.Zigbee.Protocol.Raw;

namespace Lsquared.SmartHome.Zigbee.Transports.Internals
{
    /// <summary>
    /// Represents the default reader to read data from transports.
    /// </summary>
    public sealed class DefaultTransportReader : ITransportReader
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DefaultTransportReader"/>.
        /// </summary>
        /// <param name="stream">The underlying stream of the transport.</param>
        /// <param name="extractor">The packets extractor used to read from transport.</param>
        /// <param name="encoder">The specific encoder to encode packets.</param>
        public DefaultTransportReader(Stream stream, IPacketExtractor extractor, IPacketEncoder encoder) =>
            (_reader, _extractor, _encoder) = (PipeReader.Create(stream), extractor, encoder);

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            _stoppingCts.Cancel();
            return default;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ReadOnlyMemory<byte>> ReadAsync()
        {
            var stoppingToken = _stoppingCts.Token;
            stoppingToken.Register(() => _reader.Complete());

            ReadResult result = default;
            ReadOnlySequence<byte> buffer = default;
            while (!stoppingToken.IsCancellationRequested)
            {
                Debug.WriteLine("[DEBUG] Reading packet...");
                var timeoutCts = new CancellationTokenSource(500);
                var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, timeoutCts.Token);

                try
                {
                    result = await _reader.ReadAsync(cts.Token).ConfigureAwait(false);

                    buffer = result.Buffer;
                    if (buffer.IsEmpty && result.IsCompleted)
                        break; // exit loop
                }
                catch (OperationCanceledException)
                {
                    // ignore and continue
                }

                if (buffer.Length > 0)
                {
                    SequencePosition examined, consumed;
                    while (_extractor.TryExtract(buffer, out var packet, out examined, out consumed))
                    {
                        yield return _encoder.Decode(packet);

                        _reader.AdvanceTo(consumed, examined);

                        if (stoppingToken.IsCancellationRequested)
                            break;

                        buffer = buffer.Slice(consumed);
                        if (buffer.Length == 0)
                            break;
                    }

                    if (buffer.Length > 0)
                        _reader.AdvanceTo(consumed, examined);
                }
            }
            Debug.WriteLine("[DEBUG] TransportPacketReader::ReadPacketsAsync terminates.");
        }

        private readonly CancellationTokenSource _stoppingCts = new();
        private readonly PipeReader _reader;
        private readonly IPacketExtractor _extractor;
        private readonly IPacketEncoder _encoder;
    }
}
