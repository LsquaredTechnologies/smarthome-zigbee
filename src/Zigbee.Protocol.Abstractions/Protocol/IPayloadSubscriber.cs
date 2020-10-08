using System;

namespace Lsquared.SmartHome.Zigbee.Protocol
{
    public interface IPayloadSubscriber
    {
        IDisposable Subscribe(IPayloadListener listener);
    }
}
