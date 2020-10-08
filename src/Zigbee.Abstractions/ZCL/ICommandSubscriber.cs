using System;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public interface ICommandSubscriber
    {
        IDisposable Subscribe(ICommandListener listener);
    }
}
