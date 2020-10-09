namespace Lsquared.SmartHome.Zigbee.Transports.Serial
{
    public interface ISerialTransportBuilder : ITransportBuilder
    {
        ISerialTransportBuilder WithAutoDicovery<TPortMatcher>() where TPortMatcher : ISerialPortAutoDiscovery, new();

    }
}
