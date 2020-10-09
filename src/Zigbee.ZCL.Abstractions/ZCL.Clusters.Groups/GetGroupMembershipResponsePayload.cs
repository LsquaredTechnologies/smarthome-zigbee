namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups
{
    public sealed record GetGroupMembershipResponsePayload(byte Capacity, Array<NWK.GroupAddress> Addresses) : CommandPayload
    {
        public override string ToString() =>
            $"Capacity: {Capacity}, {Addresses}";
    }
}
