using System.Linq;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetDevicesResponsePayload(Array<DeviceResponsePayload> Devices) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType().Name}> Devices:\n" + string.Join("\n", Devices.Cast<DeviceResponsePayload>());
    }
}
