namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetNodeDescriptorRequestPayload(NWK.Address NwkAddr) : ICommandPayload;
}
