namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    // 0x8031
    public sealed record GetNeighborTableResponsePayload(byte Status, byte Capacity, byte StartIndex, Array<NeighborTableEntry> NeighborTableEntries) : ICommandPayload
    {
        public override string ToString() =>
           $"<{GetType()}> Status: {Status:X2}, Capacity: {Capacity}, StartIndex: {StartIndex}"; // TODO show entries
    }
}
