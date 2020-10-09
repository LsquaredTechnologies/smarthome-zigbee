using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee.Extensibility
{
    public interface IExtensionCollection<T> : ICollection<IExtension<T>> where T : notnull, IExtensibleObject<T>
    {
        TExtension Find<TExtension>() where TExtension : notnull, IExtension<T>;

        IReadOnlyCollection<TExtension> FindAll<TExtension>() where TExtension : notnull, IExtension<T>;
    }
}
