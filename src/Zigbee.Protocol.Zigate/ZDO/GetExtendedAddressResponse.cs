namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetExtendedAddressResponse : Response
    {
        public GetExtendedAddressResponse(CommandHeader header, GetExtendedAddressResponsePayload payload)
            : base(header, payload) { }
    }
}
