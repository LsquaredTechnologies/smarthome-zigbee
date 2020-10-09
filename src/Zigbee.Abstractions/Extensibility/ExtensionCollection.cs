using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lsquared.SmartHome.Zigbee.Extensibility
{
    public sealed class ExtensionCollection<T> : Collection<IExtension<T>>, IExtensionCollection<T> where T : notnull, IExtensibleObject<T>
    {
        public ExtensionCollection(T owner) => _owner = owner;

        public TExtension Find<TExtension>() where TExtension : notnull, IExtension<T>
        {
            foreach (var item in this.ToArray())
                if (item is TExtension e)
                    return e;
            return default!;
        }

        public IReadOnlyCollection<TExtension> FindAll<TExtension>() where TExtension : notnull, IExtension<T>
        {
            var extensions = new List<TExtension>((int)(Count * .25));
            foreach (var item in this.ToArray())
                if (item is TExtension e)
                    extensions.Add(e);
            return extensions;
        }

        protected override void InsertItem(int index, IExtension<T> item)
        {
            base.InsertItem(index, item);
            item.Attach(_owner);
        }

        protected override void RemoveItem(int index)
        {
            var extension = this[index];
            base.RemoveItem(index);
            extension.Detach(_owner);
        }

        private readonly T _owner;
    }
}
