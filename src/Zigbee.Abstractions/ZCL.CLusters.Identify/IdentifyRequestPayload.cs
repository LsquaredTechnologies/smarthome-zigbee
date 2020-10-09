using System;

namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Identify
{
    public sealed record IdentifyRequestPayload(TimeSpan duration) : CommandPayload;
}
