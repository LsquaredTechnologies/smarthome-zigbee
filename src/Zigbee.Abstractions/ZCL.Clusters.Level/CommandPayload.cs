namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Level
{
    public abstract record CommandPayload : ICommandPayload
    {
        public ushort ClusterID => 0x0008;
    }
}
