using Lsquared.SmartHome.Zigbee.Protocol.Zigate;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.OnOff;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record OnOffRequest : BaseOnOffRequest
    {
        public OnOffRequest(APP.Address address, APP.Endpoint endpoint, OnOff onOff)
            : base(address, endpoint, onOff) { }

        public OnOffRequest(ICommand command)
            : base(command) { }
    }
}
