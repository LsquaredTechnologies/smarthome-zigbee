namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public interface ICommand : Zigbee.ICommandPayload
    {
        public APP.Address Address { get; }
        public APP.Endpoint SrcEndpoint { get; }
        public APP.Endpoint DstEndpoint { get; }
        public object Payload { get; }
    }
}
