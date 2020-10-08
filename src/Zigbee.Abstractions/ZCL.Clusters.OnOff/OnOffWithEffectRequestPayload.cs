namespace Lsquared.SmartHome.Zigbee.ZCL.Clusters.OnOff
{
    public sealed record OnOffWithEffectRequestPayload(OnOffEffect Effect, byte EffectVariant) : CommandPayload;
}
