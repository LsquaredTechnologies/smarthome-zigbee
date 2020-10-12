using System;
using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Identify
{
    public sealed record IdentifyRequest : Request
    {
        public IdentifyRequest(NWK.Address dstNwkAddr, TimeSpan duration)
            : this(new Command<IdentifyRequestPayload>(
                new APP.Address(dstNwkAddr),
                1,
                1,
                new IdentifyRequestPayload(duration)))
        { }

        public IdentifyRequest(NWK.Address dstNwkAddr, APP.Endpoint dstEndpoint, TimeSpan duration)
            : this(new Command<IdentifyRequestPayload>(
                new APP.Address(dstNwkAddr),
                1,
                dstEndpoint,
                new IdentifyRequestPayload(duration)))
        { }

        public IdentifyRequest(NWK.Address dstNwkAddr, IdentifyRequestPayload payload)
            : this(new Command<IdentifyRequestPayload>(
                new APP.Address(dstNwkAddr),
                1,
                1,
                payload))
        { }

        public IdentifyRequest(NWK.Address dstNwkAddr, APP.Endpoint dstEndpoint, IdentifyRequestPayload payload)
            : this(new Command<IdentifyRequestPayload>(
                new APP.Address(dstNwkAddr),
                1,
                dstEndpoint,
                payload))
        { }

        public IdentifyRequest(Command<IdentifyRequestPayload> payload)
            : base(new ZigateCommandHeader(0x0070, 0), payload) { }
    }
}
