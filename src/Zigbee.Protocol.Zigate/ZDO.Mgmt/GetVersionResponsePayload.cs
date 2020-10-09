using System;
using Lsquared.SmartHome.Buffers;

namespace Lsquared.SmartHome.Zigbee.ZDO.Mgmt
{
    // 0x8010
    public sealed record GetVersionResponsePayload(ushort ZigbeeVersion, ushort SdkVersion) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType().Name}> Zigbee Protocol Version: {ZigbeeVersion:X4}, Zigate Sdk Version: {SdkVersion:X4}";
    }
}
