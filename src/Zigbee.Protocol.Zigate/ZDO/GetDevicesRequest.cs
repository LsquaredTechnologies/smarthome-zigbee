using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetDevicesRequest : Request
    {
        public GetDevicesRequest()
            : base(new ZigateCommandHeader(0x0015, 0), ICommandPayload.None) { }

    }
}
