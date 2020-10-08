using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    // 0x8001
    public sealed record GetExtendedAddressResponsePayload(byte Status, MAC.Address ExtAddr, NWK.Address NwkAddr, byte Count, byte StartIndex, IReadOnlyList<NWK.Address> NwkAddresses) : ICommandPayload
    {
    }
}
