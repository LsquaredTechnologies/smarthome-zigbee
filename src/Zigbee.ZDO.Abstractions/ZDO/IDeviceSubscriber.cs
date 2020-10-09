using System;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public interface IDeviceSubscriber
    {
        IDisposable Subscribe(IDeviceListener listener);
    }
}
