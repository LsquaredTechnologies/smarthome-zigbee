namespace Lsquared.SmartHome.Zigbee.Protocol
{
    public interface IPayloadListener
    {
        void OnNext(ICommandPayload payload);
    }
}
