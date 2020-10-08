using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    // 0x8006
    public sealed record GetMatchDescriptorResponsePayload(byte Status, NWK.Address NwkAddr, Array<APP.Endpoint> MatchList) : ICommandPayload
    {
    }
}
