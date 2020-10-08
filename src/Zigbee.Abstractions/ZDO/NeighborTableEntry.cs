namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record NeighborTableEntry(ulong ExtPanID, MAC.Address ExtAddr, byte BitsFlag0, byte BitsFlag1, byte Depth, byte LinkQuality) : IValue
    {
    }
}
