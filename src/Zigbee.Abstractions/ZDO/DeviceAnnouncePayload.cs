namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record DeviceAnnouncePayload(NWK.Address NwkAddr, MAC.Address ExtAddr, byte MacCapabilities, bool Rejoin) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType()}> NwkAddr: {NwkAddr:X4}, ExtAddr: {ExtAddr:X16}, MacCapabilities: {MacCapabilities:X2}, Rejoin: {Rejoin}";
    }
}
