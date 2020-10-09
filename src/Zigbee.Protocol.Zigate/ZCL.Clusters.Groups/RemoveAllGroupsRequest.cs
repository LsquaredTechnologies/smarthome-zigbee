using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups
{
    public sealed record RemoveAllGroupsRequest : Request
    {
        public RemoveAllGroupsRequest(APP.Endpoint dstEndpoint, NWK.Address dstNwkAddr)
            : base(
                  new ZigateCommandHeader(0x0064, 0),
                  new Command<RemoveAllGroupsRequestPayload>(
                      new APP.Address(dstNwkAddr),
                      dstEndpoint,
                      dstEndpoint,
                      new RemoveAllGroupsRequestPayload()))
        { }
    }
}
