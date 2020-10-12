using System;

namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Color
{
    public sealed record MoveToColorRequestPayload(ushort ColorX, ushort ColorY, TimeSpan TransitionTime) : CommandPayload
    {
        public MoveToColorRequestPayload(Color color, TimeSpan transitionTime)
            : this(ColorConverter.Convert(color), transitionTime) { }

        public MoveToColorRequestPayload(CieColor color, TimeSpan transitionTime)
            : this((ushort)(65535 * color.X), (ushort)(65535 * color.Y), transitionTime) { }

    }
}
