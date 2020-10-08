namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetActiveEndpointsResponse : Response
    {
        public GetActiveEndpointsResponse(CommandHeader header, GetActiveEndpointsResponsePayload payload)
            : base(header, payload) { }
    }
}
