namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups
{
    public sealed record AddGroupRequestPayload(NWK.GroupAddress GrpAddr) : CommandPayload;
}
