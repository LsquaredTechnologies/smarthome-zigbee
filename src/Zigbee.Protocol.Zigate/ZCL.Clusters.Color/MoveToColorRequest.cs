using System;
using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Color
{
    public sealed record MoveToColorRequest : Request
    {
        public MoveToColorRequest(NWK.Address nwkAddr, APP.Endpoint endpoint, Color color, TimeSpan transitionTime)
            : this(
                  new Command<MoveToColorRequestPayload>(
                      new APP.Address(nwkAddr),
                      endpoint,
                      endpoint,
                      new MoveToColorRequestPayload(
                          color,
                          transitionTime)))
        { }

        public MoveToColorRequest(ICommand command)
            : base(new ZigateCommandHeader(0x00B8, 0), command) { }
    }
}
