using System;
using System.Collections.Generic;
using Lsquared.SmartHome.Zigbee.ZDO;

namespace Lsquared.SmartHome.Zigbee.APP
{
    public sealed record Node
    {
        public MAC.Address ExtAddr { get; }

        public NWK.Address NwkAddr { get; internal set; }

        public NodeDescriptor Info { get; internal set; } = new NodeDescriptor();

        public PowerDescriptor PowerInfo { get; internal set; } = new PowerDescriptor(0);

        ////public ComplexDescriptor? ComplexInfo { get; internal set; }

        public ReadOnlyMemory<byte> UserInfo { get; internal set; } = EmptyUserInfo;

        public IReadOnlyDictionary<Endpoint, NodeEndpoint> Endpoints => _endpoints;

        internal ZigbeeNetwork Network { get; }

        internal Node(ZigbeeNetwork network, MAC.Address extAddr)
        {
            Network = network;
            ExtAddr = extAddr;
        }

        public NodeEndpoint? GetEndpoint(Endpoint endpoint)
        {
            _endpoints.TryGetValue(endpoint, out var ne);
            return ne;
        }

        internal void Register(Array<Endpoint> endpoints)
        {
            foreach (var endpoint in endpoints)
                if (!_endpoints.TryGetValue(endpoint, out var nodeEndpoint))
                    _endpoints.Add(endpoint, new NodeEndpoint(Network, this, endpoint));
        }

        private readonly Dictionary<Endpoint, NodeEndpoint> _endpoints = new();
        private static readonly ReadOnlyMemory<byte> EmptyUserInfo = new byte[0];
    }
}
