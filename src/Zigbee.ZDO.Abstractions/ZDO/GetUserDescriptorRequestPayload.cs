namespace Lsquared.SmartHome.Zigbee.ZDO
{
    // 0x0011
    public sealed record GetUserDescriptorRequestPayload(NWK.Address NwkAddr) : ICommandPayload
    {
    }
}
