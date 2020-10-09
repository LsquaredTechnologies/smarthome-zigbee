namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.OnOff
{
    public sealed record ToggleRequest : BaseOnOffRequest
    {
        public ToggleRequest(APP.Address address)
            : base(address, 1, OnOff.Toggle) { }

        public ToggleRequest(NWK.Address nwkAddr)
            : base(new APP.Address(nwkAddr), 1, OnOff.Toggle) { }

        public ToggleRequest(NWK.GroupAddress grpAddr)
            : base(new APP.Address(grpAddr), 1, OnOff.Toggle) { }

        public ToggleRequest(NWK.Address nwkAddr, APP.Endpoint endpoint)
            : base(new APP.Address(nwkAddr), endpoint, OnOff.Toggle) { }

        public ToggleRequest(NWK.GroupAddress grpAddr, APP.Endpoint endpoint)
            : base(new APP.Address(grpAddr), endpoint, OnOff.Toggle) { }
    }
}
