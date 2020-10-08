using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lsquared.SmartHome.Zigbee
{
    public sealed class Array<T> : IArrayValue
    {
        public int Count => _items.Count;

        public T this[int index] => _items[index];

        public Array() => _items = new List<T>();

        public Array(IEnumerable<T> list) => _items = list.ToList();

        public Array(IReadOnlyList<T> list) => _items = list;

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        public override string ToString() =>
            "[" + string.Join(",", _items.Select(e => e!.ToString())) + "]";

        private readonly IReadOnlyList<T> _items;
    }
}
