namespace Lsquared.SmartHome.Zigbee.ZDO
{
    // 0x0004
    public sealed record GetSimpleDescriptorRequestPayload(NWK.Address NwkAddr, APP.Endpoint Endpoint) : ICommandPayload
    {
    }
}
