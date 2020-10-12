using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    public sealed record GetVersionRequest : Request
    {
        public override ushort ExpectedResponseCode => 0x8010;

        public GetVersionRequest() : base(new ZigateCommandHeader(0x0010, 0), ICommandPayload.None) { }
    }
}
