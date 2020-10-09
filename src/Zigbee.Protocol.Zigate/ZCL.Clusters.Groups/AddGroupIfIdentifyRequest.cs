using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups
{
    public sealed record AddGroupIfIdentifyRequest : Request
    {
        public AddGroupIfIdentifyRequest(APP.Endpoint dstEndpoint, NWK.Address dstNwkAddr, NWK.GroupAddress grpAddr)
            : this(new Command<AddGroupIfIdentifyRequestPayload>(
                new APP.Address(dstNwkAddr),
                dstEndpoint,
                dstEndpoint,
                new AddGroupIfIdentifyRequestPayload(grpAddr)))
        { }

        internal AddGroupIfIdentifyRequest(ICommand payload)
            : base(new ZigateCommandHeader(0x0060, 0), payload) { }

        public AddGroupIfIdentifyRequest(Command<AddGroupIfIdentifyRequestPayload> payload)
            : base(new ZigateCommandHeader(0x0060, 0), payload) { }
    }
}
