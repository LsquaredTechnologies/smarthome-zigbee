namespace Lsquared.SmartHome.Zigbee.ZDO
{
    // 0x0010
    public sealed record GetComplexDescriptorRequestPayload(NWK.Address NwkAddr) : ICommandPayload
    {
    }
}
