namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetPowerDescriptorResponsePayload(byte Status, NWK.Address NwkAddr, PowerDescriptor PowerDescriptor) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType()}> Status: {Status:X2}, NwkAddr: {NwkAddr:X4}, PowerDescriptor: {PowerDescriptor}";
    }
}
