using Lsquared.SmartHome.Zigbee.Protocol.Zigate;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.OnOff;

namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public sealed record OffWithEffectRequest : Request
    {
        public OffWithEffectRequest(NWK.Address nwkAddr, OnOffEffect effect, byte effectVariant = 0)
            : this(new APP.Address(nwkAddr), 1, effect, effectVariant) { }

        public OffWithEffectRequest(NWK.GroupAddress grpAddr, OnOffEffect effect, byte effectVariant = 0)
            : this(new APP.Address(grpAddr), 1, effect, effectVariant) { }

        public OffWithEffectRequest(NWK.Address nwkAddr, APP.Endpoint endpoint, OnOffEffect effect, byte effectVariant = 0)
            : this(new APP.Address(nwkAddr), endpoint, effect, effectVariant) { }

        public OffWithEffectRequest(NWK.GroupAddress grpAddr, APP.Endpoint endpoint, OnOffEffect effect, byte effectVariant = 0)
            : this(new APP.Address(grpAddr), endpoint, effect, effectVariant) { }

        public OffWithEffectRequest(APP.Address address, APP.Endpoint endpoint, OnOffEffect effect, byte effectVariant = 0)
            : base(
                  new ZigateCommandHeader(0x0094, 0),
                  new Command<OnOffWithEffectRequestPayload>(
                      address,
                      endpoint,
                      endpoint,
                      new OnOffWithEffectRequestPayload(effect, effectVariant)))
        { }

        public OffWithEffectRequest(ICommand command)
            : base(new ZigateCommandHeader(0x0094, 0), command) { }
    }
}
