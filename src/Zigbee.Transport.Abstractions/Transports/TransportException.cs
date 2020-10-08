using System;

namespace Lsquared.SmartHome.Zigbee.Transports
{
    /// <summary>
    /// Represents an exception which occurs inside transports.
    /// </summary>
    public abstract class TransportException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TransportException"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public TransportException(string message, Exception innerException) : base(message, innerException) { }
    }
}
