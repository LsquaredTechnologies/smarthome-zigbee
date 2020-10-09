namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record ReadAttributesRequestPayload(ushort ClusterID, ushort AttributeID) : CommandPayload(ClusterID);
}
