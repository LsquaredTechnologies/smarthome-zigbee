namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetNodeDescriptorResponse : Response
    {
        public GetNodeDescriptorResponse(CommandHeader header, GetNodeDescriptorResponsePayload payload)
            : base(header, payload) { }
    }
}
