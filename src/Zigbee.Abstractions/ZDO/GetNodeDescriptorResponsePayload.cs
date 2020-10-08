namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetNodeDescriptorResponsePayload(byte Status, NWK.Address NwkAddr, NodeDescriptor NodeDescriptor) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType()}> Status: {Status:X2}, NwkAddr: {NwkAddr:X4}, NodeDescriptor: {NodeDescriptor}";
    }
}
