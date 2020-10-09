using System;
using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee.Transports.Serial.Internals.OSX
{
    internal sealed class SerialPortDiscoverer : ISerialPortDiscoverer
    {
        public IEnumerable<Internals.SerialPortInfo> Discover() =>
            throw new NotSupportedException("OSX plateform is not supported");
    }
}
