using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Lsquared.SmartHome.Zigbee.Transports.Serial.Internals.Linux
{
    internal sealed class SerialPortInfo : Internals.SerialPortInfo
    {
        public SerialPortInfo(string device) : base(device)
        {
            string? devicePath = null;
            string? subsystem = null;

            var path = $"/sys/class/tty/{Name}/device";
            if (Directory.Exists(path))
            {
                devicePath = RealPath(path);
                subsystem = Path.GetFileName(RealPath(Path.Join(devicePath, "subsystem")));
            }

            var usbInterfacePath = subsystem == "usb-serial"
                ? Path.GetDirectoryName(devicePath)
                : subsystem == "usb"
                    ? devicePath
                    : null;

            if (usbInterfacePath is not null)
            {
                var usbDevicePath = Path.GetDirectoryName(usbInterfacePath);
                if (int.TryParse(File.ReadAllText(Path.Join(usbDevicePath, "idVendor")).Trim(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var vid))
                    VID = vid;
                if (int.TryParse(File.ReadAllText(Path.Join(usbDevicePath, "idProduct")).Trim(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var pid))
                    PID = pid;

                var serialPath = Path.Join(usbDevicePath, "serial");
                if (File.Exists(serialPath))
                    Serial = File.ReadAllText(serialPath).Trim();

                Location = Path.GetFileName(usbDevicePath);

                Manufacturer = File.ReadAllText(Path.Join(usbDevicePath, "manufacturer")).Trim();
                Product = File.ReadAllText(Path.Join(usbDevicePath, "product")).Trim();

                var interfacePath = Path.Join(devicePath, "interface");
                if (File.Exists(interfacePath))
                    Interface = File.ReadAllText(interfacePath).Trim();
            }

            if (subsystem == "usb" || subsystem == "usb-serial")
                base.ApplyUsbInfo();
            else if (subsystem == "pnp")
            {
                Description = Name;
                HWID = File.ReadAllText(Path.Join(device, "id")).Trim();
            }
            else if (subsystem == "amba")
            {
                Description = Name;
                HWID = Path.GetFileName(devicePath)!;
            }
        }

        private static string RealPath(string path)
        {
            var resolved = new StringBuilder(1024);
            realpath(path, resolved);
            return resolved.ToString();
        }
#pragma warning disable IDE1006
        [DllImport("c", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr realpath(
            [MarshalAs(UnmanagedType.LPStr)] string path,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder resolved);
#pragma warning restore IDE1006
    }
}
