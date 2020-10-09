namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Color
{
    public sealed record MoveToColorTemperatureRequestPayload(ushort ColorTemperatureMired, ushort TransitionTime) : CommandPayload
    {
        public MoveToColorTemperatureRequestPayload(int temperatureInKelvin, ushort transitionTime)
            : this((ushort)(1000000 / temperatureInKelvin), transitionTime) { }
    }
}
