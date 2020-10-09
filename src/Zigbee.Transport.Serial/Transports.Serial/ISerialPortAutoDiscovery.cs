using Lsquared.SmartHome.Zigbee.Transports.Serial.Internals;

namespace Lsquared.SmartHome.Zigbee.Transports.Serial
{
    public interface ISerialPortAutoDiscovery
    {
        bool Match(SerialPortInfo portInfo);
    }
}
