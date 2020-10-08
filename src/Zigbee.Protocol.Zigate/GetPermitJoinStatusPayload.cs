namespace Lsquared.SmartHome.Zigbee
{
    // 0x8014
    public sealed record GetPermitJoinStatusPayload(bool Status) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType().Name}> Status: {(Status ? "On" : "Off")}"; // TODO create a struct to hold OnOff values...
    }
}
