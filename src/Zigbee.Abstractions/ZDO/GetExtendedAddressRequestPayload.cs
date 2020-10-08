namespace Lsquared.SmartHome.Zigbee.ZDO
{
    // 0x0001
    public sealed record GetExtendedAddressRequestPayload(NWK.Address NwkAddr, byte RequestType, byte StartIndex) : ICommandPayload
    {
    }
}
