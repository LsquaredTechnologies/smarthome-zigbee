namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record CommandHeader(ushort ClusterID) : ICommandHeader;
}
