namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Color
{
    public abstract record CommandPayload : ICommandPayload
    {
        public ushort ClusterID => 0x0300;
    }
}
