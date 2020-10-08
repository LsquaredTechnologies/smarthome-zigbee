using System;
using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record SetUserDescriptorRequest : Request
    {
        public override ushort ExpectedResponseCode => 0x802B;

        public SetUserDescriptorRequest(NWK.Address nwkAddr, byte UserDescriptorLength, ReadOnlyMemory<byte> UserDescriptor)
            : base(new ZigateCommandHeader(0x002B, 0), new SetUserDescriptorRequestPayload(nwkAddr, UserDescriptorLength, UserDescriptor)) { }
    }
}
