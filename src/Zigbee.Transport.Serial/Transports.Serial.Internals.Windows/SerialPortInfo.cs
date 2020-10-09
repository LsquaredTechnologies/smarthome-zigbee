using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Lsquared.SmartHome.Zigbee.Transports.Serial.Internals.Windows
{
    internal sealed class SerialPortInfo : Internals.SerialPortInfo
    {
        public SerialPortInfo(string device) : base(device)
        {
            var options = RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline;

            var hklm = Registry.LocalMachine;
            var usbKey = hklm.OpenSubKey($@"SYSTEM\CurrentControlSet\Enum\USB");
            foreach (var usbName in usbKey.GetSubKeyNames())
            {
                var match = Regex.Match(usbName, "VID_(?<vid>[0-9a-f]+)&PID_(?<pid>[0-9a-f]+)", options, TimeSpan.FromMilliseconds(50));
                if (match.Success)
                {
                    var deviceKey = usbKey.OpenSubKey(usbName);
                    foreach (var deviceName in deviceKey.GetSubKeyNames())
                    {
                        var subDeviceKey = deviceKey.OpenSubKey(deviceName)!;
                        var deviceParametersKey = subDeviceKey.OpenSubKey("Device Parameters");

                        var portName = deviceParametersKey.GetValue("PortName")?.ToString();
                        if (portName == device)
                        {
                            if (int.TryParse(match.Groups["vid"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var vid))
                                VID = vid;

                            if (int.TryParse(match.Groups["pid"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var pid))
                                PID = pid;

                            Name = subDeviceKey.GetValue("FriendlyName")?.ToString();
                            var hwid = subDeviceKey.GetValue("HardwareID");
                            if (hwid is string[] array)
                                HWID = array[0];
                            Location = subDeviceKey.GetValue("LocationInformation")?.ToString();
                            Description = (subDeviceKey.GetValue("DeviceDesc")?.ToString() ?? "n/a").Split(";")[^1];
                            return;
                        }
                    }
                }
            }
        }
    }
}
