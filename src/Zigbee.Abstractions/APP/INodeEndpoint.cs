namespace Lsquared.SmartHome.Zigbee.APP
{
    public interface INodeEndpoint
    {
        IClusterCollection InClusters { get; }

        IClusterCollection OutClusters { get; }
    }
}
