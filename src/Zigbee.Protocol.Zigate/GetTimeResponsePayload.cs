using System;

namespace Lsquared.SmartHome.Zigbee
{
    // 0x8017
    public sealed record GetTimeResponsePayload(DateTimeOffset DateTime) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType().Name}> Date: {DateTime}";
    }
}
