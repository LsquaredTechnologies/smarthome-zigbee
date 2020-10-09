using System;
using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee.Transports.Serial.Internals.FreeBSD
{
    internal sealed class SerialPortDiscoverer : ISerialPortDiscoverer
    {
        public IEnumerable<Internals.SerialPortInfo> Discover() =>
            throw new NotSupportedException("FreeBSD plateform is not supported");
    }
}
