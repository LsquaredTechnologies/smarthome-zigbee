namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public abstract record CommandPayload(ushort ClusterID) : ICommandPayload;
}
