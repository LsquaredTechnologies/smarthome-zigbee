using Lsquared.SmartHome.Zigbee.Transports.Serial;
using Lsquared.SmartHome.Zigbee.Transports.Serial.Internals;

namespace Lsquared.SmartHome.Zigbee
{
    public sealed class ZigateSerialPortAutoDiscovery : ISerialPortAutoDiscovery
    {
        public bool Match(SerialPortInfo port) =>
            (port.VID == 0x10C4 && port.PID == 0xEA60) ||       // New PCB
            (port.VID == 0x067B && port.PID == 0x2303);         // Old PCB
    }
}
