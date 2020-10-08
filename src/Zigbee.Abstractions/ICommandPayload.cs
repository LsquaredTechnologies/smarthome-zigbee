using System;

namespace Lsquared.SmartHome.Zigbee
{
    public interface ICommandPayload 
    {
        public static readonly ICommandPayload None = new NoCommandPayload();

        //void WriteTo(ref Span<byte> span, ref int offset, ref byte checksum);
    }
}
