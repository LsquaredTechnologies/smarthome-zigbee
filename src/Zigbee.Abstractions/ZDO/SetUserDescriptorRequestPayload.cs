using System;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    // 0x0014
    public sealed record SetUserDescriptorRequestPayload(NWK.Address NwkAddr, byte UserDescriptorLength, ReadOnlyMemory<byte> UserDescriptor) : ICommandPayload
    {
    }
}
