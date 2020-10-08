using Lsquared.SmartHome.Zigbee.Protocol.Zigate;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Level;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record MoveLevelRequest : Request
    {
        public MoveLevelRequest(NWK.Address nwkAddr, APP.Endpoint endpoint, bool withOnOffCluster, Direction direction, byte rate)
            : base(
                  new ZigateCommandHeader(0x0080, 0),
                  new Command<MoveLevelRequestPayload>(
                      new APP.Address(nwkAddr),
                      endpoint,
                      endpoint,
                      new MoveLevelRequestPayload(
                          withOnOffCluster,
                          direction,
                          rate)))
        { }

        public MoveLevelRequest(ICommand command)
            : base(new ZigateCommandHeader(0x0080, 0), command) { }
    }
}
