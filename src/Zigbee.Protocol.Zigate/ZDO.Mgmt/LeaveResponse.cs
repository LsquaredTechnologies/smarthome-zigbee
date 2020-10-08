namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    public sealed record LeaveResponse : Response
    {
        public LeaveResponse(CommandHeader header, LeaveResponsePayload payload)
            : base(header, payload) { }
    }
}
