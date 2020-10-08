namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetMatchDescriptorResponse : Response
    {
        public GetMatchDescriptorResponse(CommandHeader header, GetMatchDescriptorResponsePayload payload)
            : base(header, payload) { }
    }
}
