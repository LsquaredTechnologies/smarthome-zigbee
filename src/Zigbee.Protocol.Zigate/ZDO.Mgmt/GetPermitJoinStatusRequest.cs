using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    public sealed record GetPermitJoinStatusRequest : Request
    {
        public GetPermitJoinStatusRequest() 
            : base(new ZigateCommandHeader(0x0014, 0), ICommandPayload.None) { }
    }
}
