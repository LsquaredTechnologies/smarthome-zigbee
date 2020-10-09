namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public interface ICommandListener
    {
        void OnNext(ICommand command);
    }
}
