namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Level
{
    public sealed record MoveToColorTemperatureRequestPayload(ushort ColorTemperatureMired, ushort TransitionTime): CommandPayload;
}
