namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetPowerDescriptorRequestPayload(NWK.Address NwkAddr) : ICommandPayload;
}
