namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    public sealed record PermitJoinResponse : Response
    {
        public PermitJoinResponse(CommandHeader header, PermitJoinResponsePayload payload)
            : base(header, payload) { }
    }
}
