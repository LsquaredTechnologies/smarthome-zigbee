using Lsquared.SmartHome.Zigbee.Protocol.Zigate;

namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.OnOff
{
    public abstract record BaseOnOffRequest : Request
    {
        public BaseOnOffRequest(APP.Address address, APP.Endpoint endpoint, OnOff onOff)
            : base(
                  new ZigateCommandHeader(0x0092, 0),
                  new Command<OnOffRequestPayload>(
                      address,
                      endpoint,
                      endpoint,
                      new OnOffRequestPayload(onOff)))
        { }

        public BaseOnOffRequest(ICommand command)
            : base(new ZigateCommandHeader(0x0092, 0), command) { }
    }
}
