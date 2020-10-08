namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    public sealed record GetNeighborTableResponse : Response
    {
        public GetNeighborTableResponse(CommandHeader header, GetNeighborTableResponsePayload payload)
            : base(header, payload) { }
    }
}
