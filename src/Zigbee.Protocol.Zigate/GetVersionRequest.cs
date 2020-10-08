using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee
{
    public sealed record GetVersionRequest : Request
    {
        public GetVersionRequest() : base(new ZigateCommandHeader(0x0010, 0), ICommandPayload.None) { }
    }
}
