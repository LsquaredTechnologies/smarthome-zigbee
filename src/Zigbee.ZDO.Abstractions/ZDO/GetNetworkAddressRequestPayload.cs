namespace Lsquared.SmartHome.Zigbee.ZDO
{
    // 0x0000
    public sealed record GetNetworkAddressRequestPayload(MAC.Address ExtAddr, byte RequestType, byte StartIndex) : ICommandPayload
    {
    }
}
