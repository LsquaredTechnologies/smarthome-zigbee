namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.OnOff
{
    public sealed record OnRequest : BaseOnOffRequest
    {
        public OnRequest(APP.Address address)
            : base(address, 1, OnOff.On) { }

        public OnRequest(NWK.Address nwkAddr)
            : base(new APP.Address(nwkAddr), 1, OnOff.On) { }

        public OnRequest(NWK.GroupAddress grpAddr)
            : base(new APP.Address(grpAddr), 1, OnOff.On) { }

        public OnRequest(NWK.Address nwkAddr, APP.Endpoint endpoint)
            : base(new APP.Address(nwkAddr), endpoint, OnOff.On) { }

        public OnRequest(NWK.GroupAddress grpAddr, APP.Endpoint endpoint)
            : base(new APP.Address(grpAddr), endpoint, OnOff.On) { }
    }
}
