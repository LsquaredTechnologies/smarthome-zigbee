namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetSimpleDescriptorResponsePayload(byte Status, NWK.Address NwkAddr, SimpleDescriptor SimpleDescriptor) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType()}> Status: {Status:X2}, NwkAddr: {NwkAddr:X4}, SimpleDescriptor: {SimpleDescriptor}";
    }
}
