using System;
using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.OnOff
{
    public sealed record OnWithTimedOffRequest : Request
    {
        public OnWithTimedOffRequest(NWK.Address nwkAddr, TimeSpan onDuration, TimeSpan offDuration)
            : this(new APP.Address(nwkAddr), 1, onDuration, offDuration) { }

        public OnWithTimedOffRequest(NWK.GroupAddress grpAddr, TimeSpan onDuration, TimeSpan offDuration)
            : this(new APP.Address(grpAddr), 1, onDuration, offDuration) { }

        public OnWithTimedOffRequest(NWK.Address nwkAddr, APP.Endpoint endpoint, TimeSpan onDuration, TimeSpan offDuration)
            : this(new APP.Address(nwkAddr), endpoint, onDuration, offDuration) { }

        public OnWithTimedOffRequest(NWK.GroupAddress grpAddr, APP.Endpoint endpoint, TimeSpan onDuration, TimeSpan offDuration)
            : this(new APP.Address(grpAddr), endpoint, onDuration, offDuration) { }

        public OnWithTimedOffRequest(APP.Address address, APP.Endpoint endpoint, TimeSpan onDuration, TimeSpan offDuration)
            : base(
                  new ZigateCommandHeader(0x0093, 0),
                  new Command<OnOffWithTimedOffRequestPayload>(
                      address,
                      endpoint,
                      endpoint,
                      new OnOffWithTimedOffRequestPayload(
                          false,
                          (ushort)onDuration.TotalSeconds,
                          (ushort)offDuration.TotalSeconds)))
        { }

        public OnWithTimedOffRequest(ICommand command)
            : base(new ZigateCommandHeader(0x0093, 0), command) { }
    }
}
