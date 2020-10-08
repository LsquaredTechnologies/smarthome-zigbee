namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Level
{
    public sealed record MoveToLevelRequestTimedPayload(bool WithOnOff, byte Level, byte TransitionTime) : CommandPayload;
}
