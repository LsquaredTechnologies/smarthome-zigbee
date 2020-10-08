using Lsquared.SmartHome.Zigbee.Protocol.Zigate;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record ReadAttributeRequest : Request
    {
        public ReadAttributeRequest(NWK.Address NwkAddr, APP.Endpoint SrcEndpoint, APP.Endpoint DstEndpoint, ushort ClusterID, ushort AttributeID)
            : this(new ReadAttributesRequestPayload(
                new APP.Address(NwkAddr),
                SrcEndpoint,
                DstEndpoint,
                ClusterID, 
                AttributeID))
        { }

        internal ReadAttributeRequest(ReadAttributesRequestPayload payload)
            : base(new ZigateCommandHeader(0x0100, 0), payload) { }
    }

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
