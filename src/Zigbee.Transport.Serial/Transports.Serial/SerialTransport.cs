using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Lsquared.SmartHome.Zigbee.Protocol.Raw;
using Lsquared.SmartHome.Zigbee.Transports.Internals;

namespace Lsquared.SmartHome.Zigbee.Transports.Serial
{
    /// <summary>
    /// Represents a Serial transport.
    /// </summary>
    internal sealed class SerialTransport : ITransport
    {
        /// <summary>
        /// Initialize a new instance of <see cref="SerialTransport"/>.
        /// </summary>
        /// <param name="portName"></param>
        public SerialTransport(string portName, int baudRate) =>
            _serial = new SerialPort(portName, baudRate);

        /// <inheritdoc/>
        public ITransportReader CreateReader(IPacketExtractor extractor, IPacketEncoder encoder)
        {
            if (!_serial.IsOpen) _serial.Open();
            return new DefaultTransportReader(_serial.BaseStream, extractor, encoder);
        }

        /// <inheritdoc/>
        public ITransportWriter CreateWriter(IPacketEncoder encoder)
        {
            if (!_serial.IsOpen) _serial.Open();
            return new DefaultTransportWriter(_serial.BaseStream, encoder);
        }

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            if (_isDisposed) return default;
            Debug.WriteLine("[DEBUG] SerialTransport disposing...");
            _stoppingCts.Cancel();
            _isDisposed = true;
            _serial.Dispose();
            return default;
        }

        private void CheckDispose() =>
            _ = _isDisposed ? throw new ObjectDisposedException(typeof(SerialTransport).Name) : false;

        private readonly CancellationTokenSource _stoppingCts = new();
        private readonly SerialPort _serial;
        private bool _isDisposed;
    }
}
