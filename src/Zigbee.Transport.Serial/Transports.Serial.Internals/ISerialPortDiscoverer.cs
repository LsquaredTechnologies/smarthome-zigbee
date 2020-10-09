using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee.Transports.Serial
{
    public interface ISerialPortDiscoverer
    {
        IEnumerable<Internals.SerialPortInfo> Discover();
    }
}
