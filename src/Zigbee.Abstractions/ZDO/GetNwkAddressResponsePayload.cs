using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetNwkAddressResponsePayload(byte Status, MAC.Address ExtAddr, NWK.Address NwkAddr, byte Count, byte StartIndex) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType().Name}> Status: {Status:X2}, ExtAddr: {ExtAddr:X16}, NwkAddr: {NwkAddr:X4}";
    }
}
