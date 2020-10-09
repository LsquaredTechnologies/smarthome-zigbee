using System.Runtime.InteropServices;

namespace Lsquared.SmartHome.Zigbee.Transports.Serial.Internals
{
    internal static class OSInformation
    {
        public static OSPlatform CurrentPlatform =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? OSPlatform.Windows
                : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? OSPlatform.Linux
                    : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                        ? OSPlatform.OSX
#if NETCOREAPP3_1
                        : RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)
                            ? OSPlatform.FreeBSD
                            : OSPlatform.Create("None");
#else
                        : OSPlatform.Create("None");
#endif
    }
}
