namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups
{
    public abstract record CommandPayload : ICommandPayload
    {
        public ushort ClusterID => 0x004;
    }
}
