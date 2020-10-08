namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public interface IDeviceListener
    {
        void OnNext(INode node);
    }
}
