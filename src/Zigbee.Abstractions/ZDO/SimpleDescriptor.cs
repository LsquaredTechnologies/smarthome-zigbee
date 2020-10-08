using System.Text;

namespace Lsquared.SmartHome.Zigbee.ZDO
{
    public sealed record SimpleDescriptor : IValue
    {
        public APP.Endpoint Endpoint { get; init; }
        public ushort ProfileID { get; init; }
        public ushort DeviceID { get; init; }
        public byte DeviceVersion { get; init; }
        public Array<ushort> InputClusters { get; init; } = new Array<ushort>();
        public Array<ushort> OutputClusters { get; init; } = new Array<ushort>();

        public override string ToString()
        {
            var b = new StringBuilder(1000);
            b.Append("Endpoint: ").AppendFormat("{0:X2}", Endpoint).Append(", ");
            b.Append("ProfileID: ").AppendFormat("{0:X4}", ProfileID).Append(", ");
            b.Append("DeviceID: ").AppendFormat("{0:X4}", DeviceID).Append(", ");
            b.Append("Device Version: ").AppendFormat("{0:X2}", DeviceVersion).Append(",\n");
            b.Append("- In: ").Append(InputClusters).Append(",\n");
            b.Append("- Out: ").Append(OutputClusters);
            return b.ToString();
        }
    }
}
