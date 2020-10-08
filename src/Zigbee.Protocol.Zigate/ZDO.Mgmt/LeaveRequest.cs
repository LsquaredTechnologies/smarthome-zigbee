using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    public sealed record LeaveRequest : Request
    {
        public override ushort ExpectedResponseCode => 0x804C;

        public LeaveRequest(MAC.Address dstExtAddr, byte option)
            : base(new ZigateCommandHeader(0x004C, 0), new LeaveRequestPayload(dstExtAddr, option)) { }

        public LeaveRequest(LeaveRequestPayload payload)
            : base(new ZigateCommandHeader(0x004C, 0), payload) { }
    }
}
