using System;
using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.Color
{
    public sealed record MoveToColorTemperatureRequest : Request
    {
        public MoveToColorTemperatureRequest(NWK.Address nwkAddr, APP.Endpoint endpoint, int colorTemperature, TimeSpan transitionTime)
            : base(
                  new ZigateCommandHeader(0x00C0, 0),
                  new Command<MoveToColorTemperatureRequestPayload>(
                      new APP.Address(nwkAddr),
                      endpoint,
                      endpoint,
                      new MoveToColorTemperatureRequestPayload(
                          (ushort)(1000000 / colorTemperature),
                          (ushort)(transitionTime.TotalSeconds / 10))))
        { }

        public MoveToColorTemperatureRequest(ICommand command)
            : base(new ZigateCommandHeader(0x00C0, 0), command) { }
    }
}
