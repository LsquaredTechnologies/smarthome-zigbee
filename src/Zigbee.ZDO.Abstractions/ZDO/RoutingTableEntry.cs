namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record RoutingTableEntry 
    {
        public NWK.Address DstNwkAddr { get; }

        public byte BitsFlag0 { get; }

        public NWK.Address NextHopNwkAddr { get; }
    }
}
