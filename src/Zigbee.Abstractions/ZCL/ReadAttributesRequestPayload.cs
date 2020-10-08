namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record ReadAttributesRequestPayload(APP.Address Address, APP.Endpoint SrcEndpoint, APP.Endpoint DstEndpoint, ushort ClusterID, ushort AttributeID) : ICommandPayload;
}
