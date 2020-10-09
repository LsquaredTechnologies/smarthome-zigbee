namespace Lsquared.SmartHome.Zigbee.ZDO
{
    // 0x8010
    public sealed record GetComplexDescriptorResponsePayload(byte Status, NWK.Address NwkAddr, byte ComplexDescriptorLength, ComplexDescriptor ComplexDescriptor) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType()}> Status: {Status:X2}, NwkAddr: {NwkAddr:X4}, ComplexDescriptor: {ComplexDescriptor}";
    }
}
