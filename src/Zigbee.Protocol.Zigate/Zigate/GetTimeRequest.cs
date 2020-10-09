using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.Zigate
{
    public sealed record GetTimeRequest : Request
    {
        //public override bool HasResponse => true;
        public override ushort ExpectedResponseCode => 0x8017;
        public GetTimeRequest() : base(new ZigateCommandHeader(0x0017, 0), ICommandPayload.None) { }
    }
}
