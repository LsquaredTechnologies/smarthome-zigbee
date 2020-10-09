using System.IO;

namespace Lsquared.SmartHome.Zigbee.Transports.Serial.Internals
{
    public abstract class SerialPortInfo
    {
        public string Device { get; }

        public string? Name { get; protected set; }

        public string? Description { get; protected set; }

        public string? HWID { get; protected set; }

        public int VID { get; protected set; }

        public int PID { get; protected set; }

        public string? Serial { get; protected set; }

        public string? Location { get; protected set; }

        public string? Manufacturer { get; protected set; }

        public string? Product { get; protected set; }

        public string? Interface { get; protected set; }

        protected SerialPortInfo(string device)
        {
            Device = device;
            Name = Path.GetFileName(device);
            Description = "n/a";
            HWID = "n/a";
        }

        public override string ToString() =>
            $"{Device} - {Description} - {HWID}";

        protected void ApplyUsbInfo()
        {
            Description = GetDescriptionStr();
            HWID = GetInfoStr();
        }

        private string GetDescriptionStr() =>
            Interface is not null
                ? $"{Product} - {Interface}"
                : Product is not null
                    ? Product
                    : Name is not null
                        ? Name
                        : string.Empty;

        private string GetInfoStr() =>
            $"USB {VID:X4}:{PID:X4}{GetSerialStr()}{GetLocationStr()}";

        private string GetSerialStr() =>
            Serial is null ? string.Empty : $" SER={Serial}";

        private string GetLocationStr() =>
            Location is null ? string.Empty : $" LOC={Location}";
    }
}
