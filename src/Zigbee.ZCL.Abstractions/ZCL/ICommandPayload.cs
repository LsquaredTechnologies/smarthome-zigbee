namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public interface ICommandPayload : Zigbee.ICommandPayload
    {
        ushort ClusterID { get; }
    }
}
