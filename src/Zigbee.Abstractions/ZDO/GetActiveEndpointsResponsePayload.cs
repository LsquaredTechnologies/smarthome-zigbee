using System.Linq;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record GetActiveEndpointsResponsePayload(byte Status, NWK.Address NwkAddr, Array<APP.Endpoint> ActiveEndpoints) : ICommandPayload
    {
        public override string ToString() =>
            $"<{GetType()}> Status: {Status:X2}, NwkAddr: {NwkAddr:X4}, Endpoints: {ActiveEndpoints}";
    }
}
