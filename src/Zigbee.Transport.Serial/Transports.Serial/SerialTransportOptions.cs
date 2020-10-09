namespace Lsquared.SmartHome.Zigbee.Transports.Serial
{
    public sealed class SerialTransportOptions
    {
        public bool AutoDiscover { get; set; }

        public int BaudRate { get; set; } = 115200;

        public string? PortName { get; set; }
    }
}
