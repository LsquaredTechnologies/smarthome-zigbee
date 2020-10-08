namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record DeviceResponsePayload(byte Id, NWK.Address NwkAddr, MAC.Address ExtAddr, byte PowerSource, byte LinkQuality) : IValue
    {
        public override string ToString() =>
            $"- Device#{Id:X2}: NwkAddr: {NwkAddr:X4}, ExtAddr: {ExtAddr:X16}, Power Source: {PowerSource}, Link Quality: {LinkQuality:X2}";
    }
}
