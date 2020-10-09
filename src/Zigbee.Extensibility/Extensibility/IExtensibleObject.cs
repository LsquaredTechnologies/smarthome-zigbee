using System.Diagnostics.CodeAnalysis;

namespace Lsquared.SmartHome.Zigbee.Extensibility
{
    public interface IExtensibleObject<T> where T : notnull, IExtensibleObject<T>
    {
        [NotNull]
        IExtensionCollection<T> Extensions { get; }
    }
}
