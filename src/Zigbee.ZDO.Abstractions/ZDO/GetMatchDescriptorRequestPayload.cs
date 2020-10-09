using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    // 0x0006
    public sealed record GetMatchDescriptorRequestPayload(NWK.Address NwkAddr, ushort ProfileID, Array<ushort> InputClusters, Array<ushort> OutputClusters) : ICommandPayload
    {
    }
}
