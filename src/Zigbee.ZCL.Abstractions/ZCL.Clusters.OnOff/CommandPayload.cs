namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.OnOff
{
    public abstract record CommandPayload : ICommandPayload
    {
        public ushort ClusterID => 0x006;
    }
}
