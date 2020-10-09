using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record ReadAttributeRequest : Request
    {
        public ReadAttributeRequest(NWK.Address NwkAddr, APP.Endpoint SrcEndpoint, APP.Endpoint DstEndpoint, ushort ClusterID, ushort AttributeID)
            : this(new ZCL.Command<ZCL.ReadAttributesRequestPayload>(
                new APP.Address(NwkAddr),
                SrcEndpoint,
                DstEndpoint,
                new ZCL.ReadAttributesRequestPayload(
                    ClusterID, 
                    AttributeID)))
        { }

        internal ReadAttributeRequest(ZCL.Command<ZCL.ReadAttributesRequestPayload> payload)
            : base(new ZigateCommandHeader(0x0100, 0), payload) { }
    }
}
