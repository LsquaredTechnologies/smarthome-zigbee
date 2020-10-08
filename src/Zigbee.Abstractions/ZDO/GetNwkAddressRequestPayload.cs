namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetNwkAddressRequestPayload(MAC.Address ExtAddr, byte RequestType, byte StartIndex) : ICommandPayload;
}
