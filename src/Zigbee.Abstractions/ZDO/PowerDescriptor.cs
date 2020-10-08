namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record PowerDescriptor(ushort BitsFlag) : IValue
    {
        public PowerSourceLevel CurrentPowerSourceLevel => (PowerSourceLevel)((BitsFlag & 0b1111000000000000) >> 12);

        public PowerSource CurrentPowerSource => (PowerSource)((BitsFlag & 0b0000111100000000) >> 8);

        public PowerSource AvailablePowerSource => (PowerSource)((BitsFlag & 0b0000000011110000) >> 4);

        public PowerMode CurrentPowerMode => (PowerMode)(BitsFlag & 0b0000000000001111);

        public override string ToString() =>
            BitsFlag.ToString("X4");
    }
}
