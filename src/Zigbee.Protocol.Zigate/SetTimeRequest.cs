using System;
using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee
{
    public sealed record SetTimeRequest : Request
    {
        public SetTimeRequest() : base(new ZigateCommandHeader(0x0016, 0), new SetTimeRequestPayload(DateTime.UtcNow)) { }
        public SetTimeRequest(DateTime value) : base(new ZigateCommandHeader(0x0016, 0), new SetTimeRequestPayload(value)) { }
    }
}
