using System;
using Lsquared.SmartHome.Zigbee.Protocol.Zigate;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Level;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record MoveToLevelRequest : Request
    {
        public MoveToLevelRequest(NWK.Address nwkAddr, APP.Endpoint endpoint, bool withOnOffCluster, byte level, TimeSpan transitionTime)
            : base(
                  new ZigateCommandHeader(0x0081, 0),
                  new Command<MoveToLevelRequestTimedPayload>(
                      new APP.Address(nwkAddr),
                      endpoint,
                      endpoint,
                      new MoveToLevelRequestTimedPayload(
                          withOnOffCluster,
                          level,
                          (byte)(transitionTime.TotalSeconds / 10))))
        { }

        public MoveToLevelRequest(ICommand command)
            : base(new ZigateCommandHeader(0x0081, 0), command) { }
    }
}
