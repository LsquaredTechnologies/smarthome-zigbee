using System.Linq;

namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups
{
    public sealed record GetGroupMembershipRequestPayload(Array<NWK.GroupAddress> Addresses) : CommandPayload
    {
        public GetGroupMembershipRequestPayload() : this(new Array<NWK.GroupAddress>()) { }
    }
}
