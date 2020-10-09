using System.Collections.Generic;
using System.IO.Ports;

namespace Lsquared.SmartHome.Zigbee.Transports.Serial.Internals.Windows
{
    internal sealed class SerialPortDiscoverer : ISerialPortDiscoverer
    {
        public IEnumerable<Internals.SerialPortInfo> Discover()
        {
            foreach (var portName in SerialPort.GetPortNames())
                yield return new SerialPortInfo(portName);
        }
    }
}
