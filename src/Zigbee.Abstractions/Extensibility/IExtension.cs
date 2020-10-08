using System.Diagnostics.CodeAnalysis;

namespace Lsquared.SmartHome.Zigbee.Extensibility
{
    public interface IExtension<in T> where T : notnull, IExtensibleObject<T> 
    {
        void Attach([NotNull] T owner);

        void Detach([NotNull] T owner);
    }
}
