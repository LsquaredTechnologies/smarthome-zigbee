using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    // 0x8032
    public sealed record GetRoutingTableResponsePayload(byte Status, byte RoutingTableEntryTotalCount, byte StartIndex, byte RoutingTableEntryCount, IReadOnlyList<RoutingTableEntry> RoutingTableEntries) : ICommandPayload
    {
        public override string ToString() =>
        $"<{GetType()}> Status: {Status:X2}, Total: {RoutingTableEntryTotalCount}, StartIndex: {StartIndex}"; // TODO show entries
    }
}
