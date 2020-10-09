using System;
using System.Linq;
using Lsquared.SmartHome.Zigbee.Transports.Serial.Internals;

namespace Lsquared.SmartHome.Zigbee.Transports.Serial
{
    internal sealed class SerialTransportBuilder : ISerialTransportBuilder
    {
        public SerialTransportBuilder(SerialTransportOptions options) =>
            _options = options;

        public ISerialTransportBuilder WithAutoDicovery<TSerialPortAutoDiscovery>() where TSerialPortAutoDiscovery : ISerialPortAutoDiscovery, new()
        {
            _autoDiscoverer = new TSerialPortAutoDiscovery();
            return this;
        }

        public ITransport Build()
        {
            if (_options.AutoDiscover)
            {
                if (_autoDiscoverer is null)
                    throw new InvalidOperationException("Must set AutoDiscovery service in order to used AutoDiscover mode!");

                var discoverer = SerialPortDiscovererFactory.Create();
                var port = discoverer.Discover().FirstOrDefault(_autoDiscoverer.Match);
                if (port is not null)
                    _options.PortName = port.Device;

                if (_options.PortName is null)
                    throw new InvalidOperationException("No serial port found.");
            }
            else if (_options.PortName is null)
                throw new InvalidOperationException("Must specify a valid PortName.");

            return new SerialTransport(_options.PortName, _options.BaudRate);
        }

        private readonly SerialTransportOptions _options;
        private ISerialPortAutoDiscovery? _autoDiscoverer;
    }
}
