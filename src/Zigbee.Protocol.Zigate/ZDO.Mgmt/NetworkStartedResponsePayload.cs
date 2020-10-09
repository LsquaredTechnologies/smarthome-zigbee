namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    // 0x8024
    public sealed record NetworkStartedResponsePayload(byte Status, NWK.Address NwkAddr, MAC.Address ExtAddr, NWK.Channel Channel) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType().Name}> Status: {Status:X4}, NwkAddr: {NwkAddr:X4}, ExtAddr: {ExtAddr:X16}, Channel: {Channel}";
    }
}
