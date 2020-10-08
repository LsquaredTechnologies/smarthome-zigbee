using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    // 0x8000
    public sealed record GetNetworkAddressResponsePayload(byte Status, MAC.Address ExtAddr, NWK.Address NwkAddr, byte Count, byte StartIndex, IReadOnlyList<NWK.Address> NwkAddresses) : ICommandPayload
    {
    }
}
