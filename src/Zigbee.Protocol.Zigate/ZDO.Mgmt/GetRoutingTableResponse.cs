namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    public sealed record GetRoutingTableResponse : Response
    {
        public GetRoutingTableResponse(CommandHeader header, GetRoutingTableResponsePayload payload)
            : base(header, payload) { }
    }
}
