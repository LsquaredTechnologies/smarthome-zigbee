namespace Lsquared.SmartHome.Zigbee.ZDO
{
    // 0x0005
    public sealed record GetActiveEndpointsRequestPayload(NWK.Address NwkAddr) : ICommandPayload;
}
