namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public enum PowerSourceLevel
    {
        CriticalLow = 0,
        Low = 0b0100,
        High = 0b1000,
        FullyCharged = 0b1100
    }
}
