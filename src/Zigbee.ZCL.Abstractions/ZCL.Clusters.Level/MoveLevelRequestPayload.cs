namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Level
{
    public sealed record MoveLevelRequestPayload(bool WithOnOff, Direction Direction, byte Rate) : CommandPayload;
}
