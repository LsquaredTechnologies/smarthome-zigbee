using System;

namespace Lsquared.SmartHome.Zigbee.Transports.Serial
{
    /// <summary>
    /// Represents an exception which occurs inside Serial transport.
    /// </summary>
    public sealed class SerialTransportException : TransportException
    {
        /// <summary>
        /// The serial port name or path.
        /// </summary>
        public string PortName { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="SerialTransportException"/>.
        /// </summary>
        /// <param name="portName">The serial port name or path.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SerialTransportException(string portName, string message, Exception innerException) : base(message, innerException) =>
            PortName = portName;
    }
}
