using System.Collections;

namespace Lsquared.SmartHome.Zigbee
{
    public interface IArrayValue : IEnumerable
    {
        int Count { get; }
    }
}
