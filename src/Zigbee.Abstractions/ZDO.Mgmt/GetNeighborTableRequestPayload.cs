using System;

namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    // 0x0031
    public sealed record GetNeighborTableRequestPayload(NWK.Address DstNwkAddr, byte StartIndex) : ICommandPayload;
}
