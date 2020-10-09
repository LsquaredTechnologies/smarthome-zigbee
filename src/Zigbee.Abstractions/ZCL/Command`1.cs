namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record Command<T>(APP.Address Address, APP.Endpoint SrcEndpoint, APP.Endpoint DstEndpoint, T Payload) : ICommand
        where T : notnull, ICommandPayload
    {
        ICommandPayload ICommand.Payload => Payload;

        public override string ToString() =>
            $"<{Payload.GetType().Name}> Address: {Address}, SrcEndpoint: {SrcEndpoint:X2}, DstEndpoint: {DstEndpoint:X2}, {Payload}";
    }
}
