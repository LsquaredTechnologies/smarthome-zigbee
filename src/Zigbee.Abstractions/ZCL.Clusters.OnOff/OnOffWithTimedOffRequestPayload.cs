namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.OnOff
{
    public sealed record OnOffWithTimedOffRequestPayload(bool OnlyWhenOn, ushort OnTimeInTenthOfSeconds, ushort OffTimeInTenthOfSeconds) : CommandPayload;
}
