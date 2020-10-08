namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record Command(APP.Address Address, APP.Endpoint SrcEndpoint, APP.Endpoint DstEndpoint) : ICommand
    {
        object ICommand.Payload => ICommandPayload.None;
    }
}
