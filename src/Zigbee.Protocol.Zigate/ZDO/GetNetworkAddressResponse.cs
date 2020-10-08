namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetNetworkAddressResponse : Response
    {
        public GetNetworkAddressResponse(CommandHeader header, GetNetworkAddressResponsePayload payload)
            : base(header, payload) { }
    }
}
