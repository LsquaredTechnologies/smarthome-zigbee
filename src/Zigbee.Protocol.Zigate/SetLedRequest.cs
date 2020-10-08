using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee
{
    public sealed record SetLedRequest : Request
    {
        public SetLedRequest(bool onOff) : base(new ZigateCommandHeader(0x0018, 0), new SetLedRequestPayload(onOff)) { }
    }
}
