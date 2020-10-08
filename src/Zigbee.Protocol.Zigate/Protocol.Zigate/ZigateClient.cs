////using System;
////using System.Buffers;
////using System.Diagnostics;
////using System.Threading.Tasks;
////using Lsquared.SmartHome.Zigbee.Protocol.Commands;
////using Lsquared.SmartHome.Zigbee.Transports;

////namespace Lsquared.SmartHome.Zigbee.Protocol.Zigate
////{
////    public sealed class ZigateClient : ICommandListener
////    {
////        public ZigbeeNetwork Network { get; }

////        public ZigateClient(ITransport transport)
////        {
////            Network = new ZigbeeNetwork(transport, new ZigateProtocol());
////            Network.Subscribe(this);
////        }

////        public ValueTask SendAsync(Request request) =>
////            SendAsync(Convert(request));

////        public ValueTask SendAsync(Response response) =>
////            SendAsync(Convert(response));

////        public ValueTask SendAsync(ZigateCommand command)
////        {
////            using var memoryRent = MemoryPool<byte>.Shared.Rent(2048);
////            var span = memoryRent.Memory.Span;
////            var len = command.WriteTo(ref span, 0);
////            return Network.SendAsync(memoryRent.Memory.Slice(0, len));
////        }

////        void ICommandListener.OnNext(ReadOnlyMemory<byte> raw)
////        {
////            // no op
////        }

////        void ICommandListener.OnNext(ICommand command)
////        {
////            Debug.WriteLine($"[DEBUG] Received command with payload:\n{command.Payload}");
////            switch (command.Payload)
////            {
////                //case DeviceAnnouncePayload deviceAnnounce:
////                //    //_network.Nodes.Add(deviceAnnounce.ExtAddr, deviceAnnounce.NwkAddr);
////                //    break;

////                default:
////                    // ignore 
////                    break;
////            }
////        }

////        private ZigateCommand Convert(Request request) =>
////            new ZigateCommand(
////                Convert(request.Header),
////                request.Payload);

////        private ZigateCommand Convert(Response response) =>
////            new ZigateCommand(
////                Convert(response.Header),
////                response.Payload);

////        private ZigateCommandHeader Convert(ICommandHeader header) => header switch
////        {
////            ZDO.CommandHeader h => new ZigateCommandHeader(h.ClusterID, 0, 0),
////            _ => throw new NotSupportedException()
////        };
////    }
////}
