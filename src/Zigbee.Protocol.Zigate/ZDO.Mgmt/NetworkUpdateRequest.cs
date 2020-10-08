using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    // http://ftp1.digi.com/support/images/APP_NOTE_XBee_ZigBee_Device_Profile.pdf
    public sealed record NetworkUpdateRequest : Request
    {
        public override ushort ExpectedResponseCode => 0x804A;

        public NetworkUpdateRequest(/* TODO */)
            : base(new ZigateCommandHeader(0x004A, 0), new NetworkUpdateRequestPayload(/* TODO */)) { }

        public NetworkUpdateRequest(NetworkUpdateRequestPayload payload)
            : base(new ZigateCommandHeader(0x004A, 0), payload) { }
    }
}
