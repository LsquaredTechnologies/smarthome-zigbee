using System;

namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Color
{
    public sealed record MoveToColorTemperatureRequestPayload(ushort ColorTemperatureMired, TimeSpan TransitionTime) : CommandPayload
    {
        public MoveToColorTemperatureRequestPayload(int colorTemperatureInKelvin, TimeSpan transitionTime)
            : this((ushort)(1000000 / colorTemperatureInKelvin), transitionTime) { }
    }
}
