namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    // 0x0034
    public sealed record LeaveRequestPayload(MAC.Address DstExtAddr, byte Option) : ICommandPayload;
}
