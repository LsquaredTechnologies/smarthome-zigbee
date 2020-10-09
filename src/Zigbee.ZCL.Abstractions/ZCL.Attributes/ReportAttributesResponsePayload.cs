using System.Linq;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record ReportAttributesResponsePayload(NWK.Address NwkAddr, ushort ClusterID, APP.Endpoint SrcEndpoint, APP.Endpoint DstEndpoint, Array<AttributeResponsePayload> Attributes) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType()}> NwkAddr: {NwkAddr:X4}, ClusterID: {ClusterID:X4}, SrcEndpoint: {SrcEndpoint:X2}, DstEndpoint: {DstEndpoint:X2}, Attributes:\n" +
                string.Join("\n", Attributes.Cast<AttributeResponsePayload>().Select(a => a));
    }
}
