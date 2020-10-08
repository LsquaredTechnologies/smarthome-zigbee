using System.Collections;

namespace Lsquared.SmartHome.Zigbee
{
    public interface IArrayValue : IValue, IEnumerable
    {
        int Count { get; }
    }
}
