using System.Diagnostics.CodeAnalysis;

namespace Lsquared.SmartHome.Zigbee.APP
{
    public interface IClusterCollection
    {
        [NotNull]
        ZCL.Cluster this[ushort clusterID] { get; }

        bool Contains(ushort clusterID);

        TCluster? Get<TCluster>() where TCluster : ZCL.Cluster;
    }
}
