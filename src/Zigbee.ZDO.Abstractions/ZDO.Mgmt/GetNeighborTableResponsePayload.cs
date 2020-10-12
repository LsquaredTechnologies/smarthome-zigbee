namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    // 0x8031
    public sealed record GetNeighborTableResponsePayload(byte Status, NWK.Address SrcNwkAddr, byte Capacity, byte StartIndex, Array<NeighborTableEntry> NeighborTableEntries) : ICommandPayload
    {
        public override string ToString() =>
           $"<{GetType()}> Status: {Status:X2}, SrcNwkAddr: {SrcNwkAddr:X4}, Capacity: {Capacity}, StartIndex: {StartIndex}"; // TODO show entries
    }
}
