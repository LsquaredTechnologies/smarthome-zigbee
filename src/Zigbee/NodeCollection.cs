using System.Collections;
using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee
{
    public sealed class NodeCollection : INodeCollection
    {
        public int Count => _nodesByExtAddr.Count;

        public bool Add(INode node)
        {
            if (_nodesByExtAddr.ContainsKey(node.ExtAddr))
                _nodesByNwkAddr.Remove(node.NwkAddr);

            _nodesByExtAddr.TryAdd(node.ExtAddr, node);
            return _nodesByNwkAddr.TryAdd(node.NwkAddr, node);
        }

        public bool TryGetValue(NWK.Address nwkAddr, out INode? node) =>
            _nodesByNwkAddr.TryGetValue(nwkAddr, out node);

        public bool TryGetValue(MAC.Address extAddr, out INode? node) =>
            _nodesByExtAddr.TryGetValue(extAddr, out node);

        public IEnumerator<INode> GetEnumerator() =>
            _nodesByExtAddr.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            _nodesByExtAddr.Values.GetEnumerator();

        private readonly Dictionary<MAC.Address, INode> _nodesByExtAddr = new();
        private readonly Dictionary<NWK.Address, INode> _nodesByNwkAddr = new();
    }
}
