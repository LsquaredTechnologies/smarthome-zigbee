using System.Diagnostics.CodeAnalysis;

namespace Lsquared.SmartHome.Zigbee.APP
{
    public interface INodeEndpointCollection
    {
        [NotNull]
        INodeEndpoint this[Endpoint endpoint] { get; }

        bool Contains(Endpoint endpoint);
    }
}
