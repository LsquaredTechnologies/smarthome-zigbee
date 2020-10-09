namespace Lsquared.SmartHome.Zigbee
{
    public interface INode
    {
        MAC.Address ExtAddr { get; }

        NWK.Address NwkAddr { get; }

        ////APP.INodeEndpointCollection Endpoints { get; }

        ////ZDO.NodeDescriptor Info { get; }

        ////ZDO.PowerDescriptor PowerInfo { get; }

        ////ReadOnlyMemory<byte> UserInfo { get; }
    }
}
