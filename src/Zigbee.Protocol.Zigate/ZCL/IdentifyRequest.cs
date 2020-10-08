using System;
using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record IdentifyRequest : Request
    {
        public IdentifyRequest(NWK.Address dstNwkAddr, TimeSpan duration)
            : this(dstNwkAddr, 1, duration) { }

        public IdentifyRequest(NWK.Address dstNwkAddr, APP.Endpoint dstEndpoint, TimeSpan duration)
            : base(
                  new ZigateCommandHeader(0x0070, 0),
                  new Command<ushort>(
                      new APP.Address(dstNwkAddr),
                      dstEndpoint,
                      dstEndpoint,
                      (ushort)duration.TotalSeconds))
        { }
    }
}
