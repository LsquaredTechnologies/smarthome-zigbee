using System;
using System.Runtime.InteropServices;

namespace Lsquared.SmartHome.Zigbee.Transports.Serial.Internals
{
    public static class SerialPortDiscovererFactory
    {
        public static ISerialPortDiscoverer Create() => OSInformation.CurrentPlatform switch
        {
            OSPlatform p when p == OSPlatform.Windows => new Windows.SerialPortDiscoverer(),
            OSPlatform p when p == OSPlatform.Linux => new Linux.SerialPortDiscoverer(),
            OSPlatform p when p == OSPlatform.OSX => new OSX.SerialPortDiscoverer(),
#if NETCOREAPP3_1
            OSPlatform p when p == OSPlatform.FreeBSD => new FreeBSD.SerialPortDiscoverer(),
#endif
            _ => throw new NotSupportedException("Plateform is not supported")
        };
    }
}
