namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public enum DescriptorCapabilities : byte
    {
        HasExtendedActiveEndpointsList = 0b10000000,

        HasExtendedSimpleDescriptorList = 0b01000000
    }
}
