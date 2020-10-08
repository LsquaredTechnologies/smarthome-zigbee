namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetComplexDescriptorResponse : Response
    {
        public GetComplexDescriptorResponse(CommandHeader header, GetComplexDescriptorResponsePayload payload)
            : base(header, payload) { }
    }
}
