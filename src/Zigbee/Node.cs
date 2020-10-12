using System;
using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee
{
    public sealed record Node
        : INode
        , APP.IHasNodeEndpointCollection
        , ZDO.IHasNodeDescriptor
        , ZDO.IHasPowerDescriptor
        , ZDO.IHasUserDescriptor
    {
        public MAC.Address ExtAddr { get; }

        public NWK.Address NwkAddr { get; internal set; }

        public APP.INodeEndpointCollection Endpoints { get; }

        public ZDO.NodeDescriptor NodeDescriptor { get; internal set; } = new ZDO.NodeDescriptor();

        public ZDO.PowerDescriptor PowerDescriptor { get; internal set; } = new ZDO.PowerDescriptor(0);

        public ZDO.UserDescriptor UserDescriptor { get; internal set; } = new ZDO.UserDescriptor();

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
    }
}
