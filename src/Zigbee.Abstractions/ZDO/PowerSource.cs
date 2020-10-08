using System;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    [Flags]
    public enum PowerSource
    {
        Unknown = 0,
        PermanentMains = 0b0001,
        RechargeableBattery = 0b0010,
        DisposableBattery = 0b0100,
        Reserved = 0b1000
    }
}
