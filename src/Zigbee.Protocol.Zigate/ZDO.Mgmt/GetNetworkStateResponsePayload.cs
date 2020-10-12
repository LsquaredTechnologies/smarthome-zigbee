namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    // 0x8009
    public sealed record GetNetworkStateResponsePayload(NWK.Address NwkAddr, MAC.Address ExtAddr, MAC.PanID PanId, MAC.ExtPanID ExtPanId, NWK.Channel Channel) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType().Name}> NwkAddr: {NwkAddr:X4}, ExtAddr: {ExtAddr:X16}, PanID: {PanId:X4}, ExtPanID: {ExtPanId:X16}, Channel: {Channel}";
    }
}
