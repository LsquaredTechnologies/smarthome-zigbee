using System.Collections.Generic;
using Lsquared.SmartHome.Zigbee.ZDO;

namespace Lsquared.SmartHome.Zigbee.APP
{
    public sealed class NodeEndpoint : ZCL.ICommandListener
    {
        public ushort ProfileID { get; private set; }

        public ushort DeviceID { get; private set; }

        public byte DeviceVersion { get; private set; }

        public IReadOnlyCollection<ZCL.Cluster> InputClusters => _inClusters.Values;

        public IReadOnlyCollection<ZCL.Cluster> OutputClusters => _outClusters.Values;

        internal ZigbeeNetwork Network { get; }

        internal Node Node { get; }

        internal Endpoint Endpoint { get; }

        internal NodeEndpoint(ZigbeeNetwork network, Node node, Endpoint endpoint)
        {
            Network = network;
            Node = node;
            Endpoint = endpoint;
        }

        public void OnNext(ZCL.ICommand command)
        {
            switch (command.Payload)
            {
                case ZCL.ICommandPayload cp:
                    if (_inClusters.TryGetValue(cp.ClusterID, out var cluster))
                        cluster.OnNext(cp);
                    if (_outClusters.TryGetValue(cp.ClusterID, out cluster))
                        cluster.OnNext(cp);
                    break;

                default:
                    // ignore
                    break;
            }
        }

        internal void Register(SimpleDescriptor simpleDescriptor)
        {
            ProfileID = simpleDescriptor.ProfileID;
            DeviceID = simpleDescriptor.DeviceID;
            DeviceVersion = simpleDescriptor.DeviceVersion;

            foreach (var c in simpleDescriptor.InputClusters)
            {
                var cluster = Network.CreateCluster(c);
                if (cluster is not null)
                    _inClusters.Add(c, cluster);
            }

            foreach (var c in simpleDescriptor.OutputClusters)
            {
                var cluster = Network.CreateCluster(c);
                if (cluster is not null)
                    _outClusters.Add(c, cluster);
            }
        }

        private readonly Dictionary<ushort, ZCL.Cluster> _inClusters = new();
        private readonly Dictionary<ushort, ZCL.Cluster> _outClusters = new();
    }
}
