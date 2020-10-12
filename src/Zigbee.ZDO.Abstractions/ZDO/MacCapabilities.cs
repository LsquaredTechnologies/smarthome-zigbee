using System;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    [Flags]
    public enum MacCapabilities : byte
    {
        Coordinator = 0b10000000,

        FullFunctionDevice = 0b01000000,

        MainsPowered = 0b00100000,

        RxOnWhenIdle = 0b00010000,

        HighSecurity = 0b00000010,

        AllocateAddress = 0b00000001
    }
}
