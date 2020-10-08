using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    // 0x8032
    public sealed record GetRoutingTableResponsePayload(byte Status, byte Capacity, byte StartIndex, Array<RoutingTableEntry> RoutingTableEntries) : ICommandPayload
    {
        public override string ToString() =>
        $"<{GetType()}> Status: {Status:X2}, Capacity: {Capacity}, StartIndex: {StartIndex}"; // TODO show entries
    }
}
