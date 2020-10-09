namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetDevicesRequestPayload : ICommandPayload
    {
        public override string ToString() =>
           $"<{GetType().Name}>";
    }
}
