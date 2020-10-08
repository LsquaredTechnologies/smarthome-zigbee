using System;
using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee.APP
{
    internal sealed class NodeEndpointCollection : INodeEndpointCollection
    {
        public INodeEndpoint this[Endpoint endpoint] =>
            _entries.TryGetValue(endpoint, out var nodeEndpoint)
                ? nodeEndpoint
                : throw new IndexOutOfRangeException();

        internal Node Node { get; }

        public NodeEndpointCollection(Node node) =>
            Node = node;

        public bool Contains(Endpoint enpdoint) =>
            _entries.ContainsKey(enpdoint);

        internal void Add(ZigbeeNetwork network, Node node, Endpoint endpoint) =>
            _entries.Add(endpoint, new NodeEndpoint(network, node, endpoint));

        private readonly Dictionary<Endpoint, INodeEndpoint> _entries = new();
    }
}
