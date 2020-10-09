namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Identify
{
    public abstract record CommandPayload : ICommandPayload
    {
        public ushort ClusterID => 0x0003;
    }
}
