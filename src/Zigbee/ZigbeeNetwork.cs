using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Lsquared.SmartHome.Zigbee.APP;
using Lsquared.SmartHome.Zigbee.Protocol;
using Lsquared.SmartHome.Zigbee.Protocol.Commands;
using Lsquared.SmartHome.Zigbee.Transports;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups;

namespace Lsquared.SmartHome.Zigbee
{
    public sealed class ZigbeeNetwork : ICommandSubscriber, IPayloadSubscriber, IPayloadListener, ZCL.ICommandSubscriber, ZCL.ICommandListener, IAsyncDisposable
    {
        public IReadOnlyCollection<Node> Nodes => _nodesByNwkAddr.Values;

        public Node? GetNode(NWK.Address nwkAddr) => _nodesByNwkAddr.TryGetValue(nwkAddr, out var node) ? node : node;

        public Node? GetNode(MAC.Address extAddr) => _nodesByExtAddr.TryGetValue(extAddr, out var node) ? node : node;

        public IReadOnlyCollection<NWK.GroupAddress> Groups => _groups.Keys;

        public ZigbeeNetwork(ITransport transport, IProtocol protocol)
        {
            ////_transport = transport; // keep ref to transport
            _protocol = protocol;
            _reader = transport.CreateReader(protocol.PacketExtractor, protocol.PacketEncoder);
            _writer = transport.CreateWriter(protocol.PacketEncoder);
            (_consumerThread = new Thread(Consume)).Start();
            _unsubscriberPayloadListener = Subscribe((IPayloadListener)this);
            _unsubscriberZclCommandListener = Subscribe((ZCL.ICommandListener)this);
        }

        public ValueTask DisposeAsync()
        {
            _stoppingCts.Cancel();
            _consumerThread.Join(2500);
            if (_consumerThread.IsAlive)
                _consumerThread.Abort();
            _unsubscriberPayloadListener.Dispose();
            _unsubscriberZclCommandListener.Dispose();
            return default;
        }

        public void RegisterCluster<TCluster>(ushort clusterID) where TCluster : ZCL.Cluster, new() =>
            _registeredClusters.Add(clusterID, () => new TCluster());

        internal ZCL.Cluster? CreateCluster(ushort clusterID)
        {
            if (_registeredClusters.TryGetValue(clusterID, out var factory))
                return factory();
            return null;
        }

        public async Task<ICommand?> ReceiveAsync(ushort responseCode)
        {
            var listener = new ReceiveCommandListener(_protocol.ExpectResponseCode(responseCode));
            using var unsubscriber = Subscribe(listener);
            try
            {
                var command = await listener.Result.Task; // need await to avoid `unsubscriber` to dispose!
                return command;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        public ValueTask SendAsync(ICommandPayload payload)
        {
            var request = _protocol.CreateRequest(payload);
            if (request is null) return default;
            return SendAsync(request);
        }

        public ValueTask SendAsync(Request request)
        {
            var rent = MemoryPool<byte>.Shared.Rent(2048);
            var memory = rent.Memory;
            var span = memory.Span;
            var len = _protocol.Write(request, ref span);
            return SendAsync(memory.Slice(0, len));
        }

        public ValueTask SendAsync(ReadOnlyMemory<byte> packet)
        {
            Debug.WriteLine("[DEBUG] > " + BitConverter.ToString(packet.ToArray()).Replace("-", " "));
            return _writer.WriteAsync(packet);
        }

        public async Task<ICommand?> SendAndWaitAsync(ICommandPayload payload)
        {
            var request = _protocol.CreateRequest(payload);
            if (request is null) return null;
            var receiving = ReceiveAsync(request.ExpectedResponseCode);
            await SendAsync(request);
            var response = await receiving;
            return response;
        }

        public IDisposable Subscribe(ICommandListener listener)
        {
            if (!_commandListeners.Contains(listener))
                _commandListeners.Add(listener);
            return new Unsubscriber<ICommandListener>(_commandListeners, listener);
        }

        public IDisposable Subscribe(IPayloadListener listener)
        {
            if (!_payloadListeners.Contains(listener))
                _payloadListeners.Add(listener);
            return new Unsubscriber<IPayloadListener>(_payloadListeners, listener);
        }

        public IDisposable Subscribe(ZCL.ICommandListener listener)
        {
            if (!_zclCommandListeners.Contains(listener))
                _zclCommandListeners.Add(listener);
            return new Unsubscriber<ZCL.ICommandListener>(_zclCommandListeners, listener);
        }

        void IPayloadListener.OnNext(ICommandPayload payload)
        {
            switch (payload)
            {
                case ZDO.GetDevicesResponsePayload p:
                {
                    foreach (var device in p.Devices)
                    {
                        if (_nodesByExtAddr.TryGetValue(device.ExtAddr, out var node))
                        {
                            // Existing device
                            if (node.NwkAddr != device.NwkAddr)
                            {
                                node = node with { NwkAddr = device.NwkAddr };
                                _nodesByNwkAddr.Remove(node.NwkAddr);
                                _nodesByNwkAddr.Add(device.NwkAddr, node); // replace node
                                _nodesByExtAddr[device.ExtAddr] = node; // update node
                            }
                        }
                        else
                        {
                            // Non-existing device
                            node = new Node(this, device.ExtAddr)
                            {
                                NwkAddr = device.NwkAddr,
                                PowerInfo = new ZDO.PowerDescriptor((ushort)((byte)(device.PowerSource == 0 ? ZDO.PowerSource.DisposableBattery : ZDO.PowerSource.PermanentMains) << 8))
                            };
                            // TODO device.LinkQuality?
                            _nodesByExtAddr.Add(device.ExtAddr, node);
                            _nodesByNwkAddr.TryAdd(device.NwkAddr, node);
                            Update(node);
                        }
                    }
                }
                break;

                case ZDO.DeviceAnnouncePayload p:
                {
                    if (_nodesByExtAddr.TryGetValue(p.ExtAddr, out var node))
                    {
                        // Existing device
                        if (node.NwkAddr != p.NwkAddr)
                        {
                            node = node with { NwkAddr = p.NwkAddr };
                            _nodesByNwkAddr.Remove(node.NwkAddr);
                            _nodesByNwkAddr.Add(p.NwkAddr, node); // replace node
                            _nodesByExtAddr[p.ExtAddr] = node; // update node
                        }
                    }
                    else
                    {
                        // Non-existing device
                        node = new Node(this, p.ExtAddr)
                        {
                            NwkAddr = p.NwkAddr,
                            Info = new ZDO.NodeDescriptor { MacCapabilities = p.MacCapabilities }
                        };
                        _nodesByExtAddr.Add(p.ExtAddr, node);
                        _nodesByNwkAddr.TryAdd(p.NwkAddr, node);
                    }

                    Update(node);
                }
                break;

                case ZDO.GetActiveEndpointsResponsePayload p:
                {
                    if (_nodesByNwkAddr.TryGetValue(p.NwkAddr, out var node))
                    {
                        node.Register(p.ActiveEndpoints);
                        foreach (var endpoint in p.ActiveEndpoints)
                            Enqueue(new ZDO.GetSimpleDescriptorRequestPayload(p.NwkAddr, endpoint));
                    }
                }
                break;

                case ZDO.GetNodeDescriptorResponsePayload p:
                {
                    if (_nodesByNwkAddr.TryGetValue(p.NwkAddr, out var node))
                    {
                        node.Info = p.NodeDescriptor;

                        var isRouter = (p.NodeDescriptor.MacCapabilities & 0b01000000) == 0b01000000;
                        if (isRouter)
                        {
                            Enqueue(new ZDO.Mgmt.GetRoutingTableRequestPayload(node.NwkAddr, 0));
                            Enqueue(new ZDO.Mgmt.GetNeighborTableRequestPayload(node.NwkAddr, 0));
                        }
                    }
                }
                break;

                case ZDO.GetPowerDescriptorResponsePayload p:
                {
                    if (_nodesByNwkAddr.TryGetValue(p.NwkAddr, out var node))
                        node.PowerInfo = p.PowerDescriptor;
                }
                break;

                case ZDO.GetUserDescriptorResponsePayload p:
                {
                    if (_nodesByNwkAddr.TryGetValue(p.NwkAddr, out var node))
                        node.UserInfo = p.UserDescriptor;
                }
                break;

                case ZDO.GetSimpleDescriptorResponsePayload p:
                {
                    if (_nodesByNwkAddr.TryGetValue(p.NwkAddr, out var node))
                        node.Endpoints[p.SimpleDescriptor.Endpoint].Register(p.SimpleDescriptor);
                }
                break;

                case ZDO.GetComplexDescriptorResponsePayload _:
                {
                    ////if (_nodesByNwkAddr.TryGetValue(p.NwkAddr, out var node))
                    ////    node.Register(p.ComplexDescriptor);
                }
                break;

                case ZCL.ICommand zclCommand:
                    foreach (var listener in _zclCommandListeners)
                        listener.OnNext(zclCommand);
                    break;

                default:
                    // ignore
                    break;
            }
        }

        private void Update(Node node)
        {
            Enqueue(new ZDO.GetActiveEndpointsRequestPayload(node.NwkAddr));
            //Enqueue(new ZDO.GetPowerDescriptorRequestPayload(node.NwkAddr));
            //Enqueue(new ZDO.GetNodeDescriptorRequestPayload(node.NwkAddr));
            //Enqueue(new ZDO.GetUserDescriptorRequestPayload(node.NwkAddr));
            //Enqueue(new ZDO.GetSimpleDescriptorRequestPayload(node.NwkAddr, 0));
            ////Enqueue(new ZDO.GetComplexDescriptorRequestPayload(node.NwkAddr));

            var isRouter = (node.Info.MacCapabilities & 0b01000000) == 0b01000000;
            if (isRouter)
            {
                Enqueue(new ZDO.Mgmt.GetRoutingTableRequestPayload(node.NwkAddr, 0));
                Enqueue(new ZDO.Mgmt.GetNeighborTableRequestPayload(node.NwkAddr, 0));
            }

            Enqueue(new ZCL.Command<GetGroupMembershipRequestPayload>(node.NwkAddr, 1, 1, new GetGroupMembershipRequestPayload()));

            //Enqueue(new ZDO.GetMatchDescriptorRequestPayload(node.NwkAddr, ProfileID, InputClusters, OutputClusters));
        }

        void ZCL.ICommandListener.OnNext(ZCL.ICommand command)
        {
            Node? node = default;
            var address = command.Address;

            if (address.Mode == AddressMode.Short)
            {
                if (command.Payload is Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups.GetGroupMembershipResponsePayload p)
                {
                    foreach (var grpAddr in p.Addresses)
                        _groups.TryAdd(grpAddr, string.Empty);
                }


                if (!_nodesByNwkAddr.TryGetValue(address.NwkAddr, out node))
                    Enqueue(new Lsquared.SmartHome.Zigbee.ZDO.GetActiveEndpointsRequestPayload(address.NwkAddr));
            }
            else if (address.Mode == AddressMode.IEEE)
            {
                if (!_nodesByExtAddr.TryGetValue(address.ExtAddr, out node))
                    return;
            }

            if (node is null)
                return;

            node.GetEndpoint(command.DstEndpoint)?.OnNext(command);
        }

        private async void Consume(object? obj)
        {
            Thread.CurrentThread.Name = "ZigbeeNetwork::Consume";
            var stoppingToken = _stoppingCts.Token;

            var packets = _reader.ReadAsync();
            await foreach (var packet in packets)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                Debug.WriteLine("[DEBUG] < " + BitConverter.ToString(packet.ToArray()).Replace("-", " "));
                foreach (var listener in _commandListeners.ToArray())
                    listener?.OnNext(packet);

                var command = _protocol.Read(packet);
                if (command.Payload == ICommandPayload.None)
                {
                    // TODO log information?
                    Debug.WriteLine($"Command is not handled: {command.Header}");
                    continue;
                }

                Console.WriteLine(command.Payload);

                foreach (var listener in _commandListeners.ToArray())
                    listener?.OnNext(command);

                foreach (var listener in _payloadListeners.ToArray())
                    listener?.OnNext(command.Payload);

                if (stoppingToken.IsCancellationRequested)
                    break;
            }
        }

        private async void Enqueue(ICommandPayload payload)
        {
            var request = _protocol.CreateRequest(payload);
            if (request is null) return;
            var receiving = ReceiveAsync(request.ExpectedResponseCode);
            await SendAsync(request);
            var response = await receiving;
            _ = response;
        }

        private readonly CancellationTokenSource _stoppingCts = new();
        private readonly List<ICommandListener> _commandListeners = new();
        private readonly List<IPayloadListener> _payloadListeners = new();
        private readonly List<ZCL.ICommandListener> _zclCommandListeners = new();
        private readonly Dictionary<MAC.Address, Node> _nodesByExtAddr = new();
        private readonly Dictionary<NWK.Address, Node> _nodesByNwkAddr = new();
        private readonly Dictionary<NWK.GroupAddress, string> _groups = new();
        ////private readonly ITransport _transport;
        private readonly IProtocol _protocol;
        private readonly ITransportReader _reader;
        private readonly ITransportWriter _writer;
        private readonly Thread _consumerThread;
        private readonly IDisposable _unsubscriberPayloadListener;
        private readonly IDisposable _unsubscriberZclCommandListener;
        private readonly Dictionary<ushort, Func<ZCL.Cluster>> _registeredClusters = new();

        private readonly struct Unsubscriber<TListener> : IDisposable
        {
            public Unsubscriber(IList<TListener> listeners, TListener listener) =>
                (_listeners, _listener) = (listeners, listener);

            public void Dispose()
            {
                if (_listeners.Contains(_listener))
                    _listeners.Remove(_listener);
            }

            private readonly IList<TListener> _listeners;
            private readonly TListener _listener;
        }

        private sealed class ReceiveCommandListener : ICommandListener
        {
            public TaskCompletionSource<ICommand> Result { get; private set; }

            public ReceiveCommandListener(Func<ICommand, bool> predicate)
            {
                _predicate = predicate;
                Result = new TaskCompletionSource<ICommand>();
                _cts = new(WaitTimeoutInMilliseconds);
                _cts.Token.Register(() =>
                {
                    if (!Result.Task.IsCompleted)
                        Result.TrySetCanceled();
                });
            }

            void ICommandListener.OnNext(ReadOnlyMemory<byte> raw)
            {
                // no op, but needed by contract
            }

            void ICommandListener.OnNext(ICommand command)
            {
                if (_predicate(command))
                    Result.TrySetResult(command);
            }

#if DEBUG
            private static readonly int WaitTimeoutInMilliseconds = 500;
#else
            private static readonly int WaitTimeoutInMilliseconds = 500;
#endif

            private readonly Func<ICommand, bool> _predicate;
            private readonly CancellationTokenSource _cts;
        }
    }
}
