namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    public sealed record NetworkUpdateResponse : Response
    {
        public NetworkUpdateResponse(CommandHeader header, NetworkUpdateResponsePayload payload)
            : base(header, payload) { }
    }
}
