using System;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetUserDescriptorResponsePayload(byte Status, NWK.Address NwkAddr, byte UserDescriptorLength, ReadOnlyMemory<byte> UserDescriptor) : ICommandPayload
    {
        public override string ToString() =>
           $"<{GetType()}> Status: {Status:X2}, NwkAddr: {NwkAddr:X4}, UserDescriptor: {BitConverter.ToString(UserDescriptor.ToArray()).Replace("-", "")}";
    }
}
