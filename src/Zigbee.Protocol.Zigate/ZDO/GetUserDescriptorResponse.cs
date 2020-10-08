namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetUserDescriptorResponse : Response
    {
        public GetUserDescriptorResponse(CommandHeader header, GetUserDescriptorResponsePayload payload)
            : base(header, payload) { }
    }
}
