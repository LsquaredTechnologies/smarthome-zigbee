using System;
using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee
{
    public sealed record Node : INode
    {
        public MAC.Address ExtAddr { get; }

        public NWK.Address NwkAddr { get; internal set; }

        public bool IsEndDevice { get; }

        public ZDO.NodeDescriptor Info { get; internal set; } = new ZDO.NodeDescriptor();

        public ZDO.PowerDescriptor PowerInfo { get; internal set; } = new ZDO.PowerDescriptor(0);

        ////public ZDO.ComplexDescriptor? ComplexInfo { get; internal set; }

        public ReadOnlyMemory<byte> UserInfo { get; internal set; } = EmptyUserInfo;

        public APP.INodeEndpointCollection Endpoints { get; } 

        internal ZigbeeNetwork Network { get; }

        internal Node(ZigbeeNetwork network, MAC.Address extAddr)
        {
            Endpoints = new APP.NodeEndpointCollection(this);
            Network = network;
            ExtAddr = extAddr;
        }

        internal void Register(Array<APP.Endpoint> endpoints)
        {
            foreach (var endpoint in endpoints)
                if (!Endpoints.Contains(endpoint))
                    ((APP.NodeEndpointCollection)Endpoints).Add(Network, this, endpoint);
        }

        private static readonly ReadOnlyMemory<byte> EmptyUserInfo = new byte[0];
    }
}
