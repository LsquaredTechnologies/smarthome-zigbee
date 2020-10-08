using System.Diagnostics.CodeAnalysis;

namespace Lsquared.SmartHome.Zigbee.Transports.Serial
{
    /// <summary>
    /// Represents a Serial transport factory
    /// </summary>
    /// <seealso cref="SerialTransport"/>
    public static class SerialTransportFactory
    {
        /// <summary>
        /// Creates a new transport from serial port.
        /// </summary>
        /// <param name="portName">The port name or path to open the serial port.</param>
        /// <returns>An instance of <see cref="ITransport"/>.</returns>
        [return: NotNull]
        public static ITransport FromSerial(string? portName) =>
            new SerialTransport(portName is null ? "COM5" : portName); // TODO check for hardware ports
    }
}
