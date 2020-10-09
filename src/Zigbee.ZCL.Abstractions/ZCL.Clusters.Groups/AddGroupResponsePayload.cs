namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups
{
    public sealed record AddGroupResponsePayload(APP.Endpoint Endpoint, ushort ClusterID, byte Status, NWK.GroupAddress GrpAddr, NWK.Address SrcNwkAddr) : CommandPayload
    {
        public override string ToString() =>
            $"<{GetType().Name}> Endpoint: {Endpoint}, ClusterID: {ClusterID}, Status: {Status}, GrpAddr: {GrpAddr}, SrcNwkAddr: {SrcNwkAddr}";
    }
}
