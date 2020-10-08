using System;
using Lsquared.SmartHome.Zigbee.Protocol.Raw;

namespace Lsquared.SmartHome.Zigbee.Transports
{
    /// <summary>
    /// Defines a transport.
    /// </summary>
    public interface ITransport : IAsyncDisposable
    {
        /// <summary>
        /// Creates a reader from the current transport.
        /// </summary>
        /// <param name="extractor">The packets extractor used to read from transport.</param>
        /// <param name="encoder">The specific encoder to encode packets.</param>
        /// <returns>An instance of <see cref="ITransportReader"/>.</returns>
        ITransportReader CreateReader(IPacketExtractor extractor, IPacketEncoder encoder);

        /// <summary>
        /// Creates a writer from the current transport.
        /// </summary>
        /// <param name="encoder">The specific encoder to encode packets.</param>
        /// <returns>An instance of <see cref="ITransportWriter"/>.</returns>
        ITransportWriter CreateWriter(IPacketEncoder encoder);
    }
}
