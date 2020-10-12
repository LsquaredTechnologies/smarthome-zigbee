using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee
{
    public interface INodeCollection : IReadOnlyCollection<INode>, IEnumerable<INode>
    {
        bool TryGetValue(NWK.Address nwkAddr, out INode? node);

        bool TryGetValue(MAC.Address extAddr, out INode? node);
    }
}
