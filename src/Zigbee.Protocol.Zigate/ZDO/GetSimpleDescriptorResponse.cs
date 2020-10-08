namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetSimpleDescriptorResponse : Response
    {
        public GetSimpleDescriptorResponse(CommandHeader header, GetSimpleDescriptorResponsePayload payload)
            : base(header, payload) { }
    }
}
