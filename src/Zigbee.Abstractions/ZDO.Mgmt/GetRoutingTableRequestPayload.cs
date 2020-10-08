namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    // 0x0032
    public sealed record GetRoutingTableRequestPayload(NWK.Address NwkAddr, byte StartIndex) : ICommandPayload
    {
    }
}
