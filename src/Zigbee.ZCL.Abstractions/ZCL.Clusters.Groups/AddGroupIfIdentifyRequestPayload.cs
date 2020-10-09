namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups
{
    public sealed record AddGroupIfIdentifyRequestPayload(NWK.GroupAddress GrpAddr) : CommandPayload;
}
