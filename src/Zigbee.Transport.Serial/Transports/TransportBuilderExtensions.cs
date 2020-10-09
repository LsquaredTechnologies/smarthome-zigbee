using Lsquared.SmartHome.Zigbee.Transports.Serial;

namespace Lsquared.SmartHome.Zigbee.Transports
{
    public static class TransportBuilderExtensions
    {
        public static ISerialTransportBuilder FromSerial(this ITransportBuilder builder, bool autoDiscover = false, int baudRate = 115200, string? portName = null) =>
            new SerialTransportBuilder(
                new SerialTransportOptions
                {
                    AutoDiscover = autoDiscover,
                    BaudRate = baudRate,
                    PortName = portName,
                });
    }
}
