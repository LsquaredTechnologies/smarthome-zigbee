using Lsquared.SmartHome.Zigbee.ZCL.Clusters.OnOff;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record OffRequest : BaseOnOffRequest
    {
        public OffRequest(APP.Address address)
            : base(address, 1, OnOff.Off) { }

        public OffRequest(NWK.Address nwkAddr)
            : base(new APP.Address(nwkAddr), 1, OnOff.Off) { }

        public OffRequest(NWK.GroupAddress grpAddr)
            : base(new APP.Address(grpAddr), 1, OnOff.Off) { }

        public OffRequest(NWK.Address nwkAddr, APP.Endpoint endpoint)
            : base(new APP.Address(nwkAddr), endpoint, OnOff.Off) { }

        public OffRequest(NWK.GroupAddress grpAddr, APP.Endpoint endpoint)
            : base(new APP.Address(grpAddr), endpoint, OnOff.Off) { }
    }
}
