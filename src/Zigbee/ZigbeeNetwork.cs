using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Lsquared.SmartHome.Zigbee.Extensibility;
using Lsquared.SmartHome.Zigbee.Protocol;
using Lsquared.SmartHome.Zigbee.Protocol.Commands;
using Lsquared.SmartHome.Zigbee.Transports;

namespace Lsquared.SmartHome.Zigbee
{
    public sealed class ZigbeeNetwork
        : INetwork
        , IPacketSubscriber
        , ICommandSubscriber
        , IPayloadSubscriber
        , ZDO.IDeviceSubscriber
        //, ZDO.ICommandSubscriber
        , ZCL.ICommandSubscriber
        , IExtensibleObject<ZigbeeNetwork>
    {
        public INodeCollection Nodes => _nodes;

        ////public IReadOnlyCollection<NWK.GroupAddress> Groups => _groups.Keys;

        public IExtensionCollection<ZigbeeNetwork> Extensions { get; }

        public ZigbeeNetwork(ITransport transport, IProtocol protocol)
        {
            Extensions = new ExtensionCollection<ZigbeeNetwork>(this);

            _protocol = protocol;
            _reader = transport.CreateReader(protocol.PacketExtractor, protocol.PacketEncoder);
            _writer = transport.CreateWriter(protocol.PacketEncoder);

            var taskScheduler = new LimitedConcurrencyLevelTaskScheduler(3);
            var factory = new TaskFactory(taskScheduler);
            _consumerTask = new Lazy<Task>(() => factory.StartNew(ConsumeAsync));
            _produceTask = new Lazy<Task>(() => factory.StartNew(ProduceAsync));
        }

        public ValueTask StartAsync()
        {
            var task = _consumerTask.Value;
            if (task.IsCompleted) return new ValueTask(task);

            task = _produceTask.Value;
            if (task.IsCompleted) return new ValueTask(task);

            Task.Run(async () =>
            {
                await _protocol.InitializeAsync(this).ConfigureAwait(false);

                // Get IEEE address and neighbors of coordinator!
                await SendAsync(new ZDO.GetExtendedAddressRequestPayload(NWK.Address.Coordinator, 0, 0)).ConfigureAwait(false);
                await SendAsync(new ZDO.Mgmt.GetNeighborTableRequestPayload(NWK.Address.Coordinator, 0)).ConfigureAwait(false);
            });

            return default;
        }

        public async ValueTask DisposeAsync()
        {
            _stoppingCts.Cancel();
            if (_consumerTask.IsValueCreated)
                await _consumerTask.Value.ConfigureAwait(false);
        }

        public void RegisterCluster<TCluster>(ushort clusterID) where TCluster : ZCL.Cluster, new() =>
            _registeredClusters.Add(clusterID, () => new TCluster());

        internal ZCL.Cluster? CreateCluster(ushort clusterID)
        {
            if (_registeredClusters.TryGetValue(clusterID, out var factory))
                return factory();
            return null;
        }

        #region Receive methods

        public async Task<ICommand?> ReceiveAsync(ushort responseCode, TimeSpan timeout)
        {
            var listener = new ReceiveCommandListener(timeout, _protocol.ExpectResponseCode(responseCode));
            using var unsubscriber = Subscribe(listener);
            try
            {
                var command = await listener.Result.Task.ConfigureAwait(false); // need await to avoid `unsubscriber` to dispose!
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

        #endregion

        #region Send methods

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
            _writeQueue.Add(packet);
            return default;
        }

        #endregion

        #region SendAndReceive method

        public async Task<ICommandPayload?> SendAndReceiveAsync(ICommandPayload payload, TimeSpan timeout)
        {
            var request = _protocol.CreateRequest(payload);
            if (request is null) return null;
            var receiving = ReceiveAsync(request.ExpectedResponseCode, timeout);
            await SendAsync(request).ConfigureAwait(false);
            var response = await receiving.ConfigureAwait(false);
            return response?.Payload;
        }

        #endregion

        #region Subscribe methods

        public IDisposable Subscribe(IPacketListener listener)
        {
            lock (_packetListeners)
                if (!_packetListeners.Contains(listener))
                    _packetListeners.Add(listener);
            return new Unsubscriber<IPacketListener>(_packetListeners, listener);
        }

        public IDisposable Subscribe(ICommandListener listener)
        {
            lock (_commandListeners)
                if (!_commandListeners.Contains(listener))
                    _commandListeners.Add(listener);
            return new Unsubscriber<ICommandListener>(_commandListeners, listener);
        }

        public IDisposable Subscribe(IPayloadListener listener)
        {
            lock (_payloadListeners)
                if (!_payloadListeners.Contains(listener))
                    _payloadListeners.Add(listener);
            return new Unsubscriber<IPayloadListener>(_payloadListeners, listener);
        }

        public IDisposable Subscribe(ZCL.ICommandListener listener)
        {
            lock (_zclCommandListeners)
                if (!_zclCommandListeners.Contains(listener))
                    _zclCommandListeners.Add(listener);
            return new Unsubscriber<ZCL.ICommandListener>(_zclCommandListeners, listener);
        }

        public IDisposable Subscribe(ZDO.IDeviceListener listener)
        {
            lock (_deviceAnnounceListeners)
                if (!_deviceAnnounceListeners.Contains(listener))
                    _deviceAnnounceListeners.Add(listener);
            return new Unsubscriber<ZDO.IDeviceListener>(_deviceAnnounceListeners, listener);
        }

        #endregion

        private async Task ProduceAsync()
        {
            try
            {
                var stoppingToken = _stoppingCts.Token;
                var e = _writeQueue.GetConsumingEnumerable(stoppingToken).GetEnumerator();
                while (e.MoveNext())
                {
                    var packet = e.Current;
                    Debug.WriteLine("[DEBUG] > " + BitConverter.ToString(packet.ToArray()).Replace("-", " "));
                    await _writer.WriteAsync(packet).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                // TODO log error
                _ = ex;
            }
        }

        private async Task ConsumeAsync()
        {
            try
            {
                var stoppingToken = _stoppingCts.Token;
                var packets = _reader.ReadAsync().ConfigureAwait(false);
                await foreach (var packet in packets)
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;

                    Debug.WriteLine("[DEBUG] < " + BitConverter.ToString(packet.ToArray()).Replace("-", " "));
                    foreach (var listener in _packetListeners.ToArray())
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

                    switch (command.Payload)
                    {
                        case ZDO.DeviceAnnounceIndicationPayload p: Register(p.ExtAddr, p.NwkAddr); break;

                        case ZDO.GetExtendedAddressResponsePayload p: Register(p.ExtAddr, p.NwkAddr); break;

                        case ZDO.GetNetworkAddressResponsePayload p: Register(p.ExtAddr, p.NwkAddr); break;

                        case ZDO.GetDevicesResponsePayload p:
                            foreach (var device in p.Devices)
                                Register(device.ExtAddr, device.NwkAddr);
                            break;

                        case ZCL.ICommand zclCommand:
                            foreach (var listener in _zclCommandListeners.ToArray())
                                listener.OnNext(zclCommand);
                            break;

                        default: /* ignore */ break;
                    }

                    if (stoppingToken.IsCancellationRequested)
                        break;
                }
            }
            catch (Exception ex)
            {
                // TODO log error
                _ = ex;

                _stoppingCts.Cancel();
            }
        }

        private void Register(MAC.Address extAddr, NWK.Address nwkAddr)
        {
            if (nwkAddr == NWK.Address.Invalid) return;

            var node = new Node(this, extAddr) with { NwkAddr = nwkAddr };
            _nodes.Add(node);

            foreach (var listener in _deviceAnnounceListeners.ToArray())
                listener.OnNext(node);
        }

        #region Fields

        private readonly CancellationTokenSource _stoppingCts = new();

        private readonly List<IPacketListener> _packetListeners = new();
        private readonly List<ICommandListener> _commandListeners = new();
        private readonly List<IPayloadListener> _payloadListeners = new();
        private readonly List<ZDO.IDeviceListener> _deviceAnnounceListeners = new();
        private readonly List<ZCL.ICommandListener> _zclCommandListeners = new();

        private readonly NodeCollection _nodes = new NodeCollection();

        private readonly IProtocol _protocol;
        private readonly ITransportReader _reader;
        private readonly ITransportWriter _writer;
        private readonly Lazy<Task> _consumerTask;
        private readonly Lazy<Task> _produceTask;
        private readonly BlockingCollection<ReadOnlyMemory<byte>> _writeQueue = new();

        private readonly Dictionary<ushort, Func<ZCL.Cluster>> _registeredClusters = new();

        #endregion

        #region Nested types

        private readonly struct Unsubscriber<TListener> : IDisposable
        {
            public Unsubscriber(IList<TListener> listeners, TListener listener) =>
                (_listeners, _listener) = (listeners, listener);

            public void Dispose()
            {
                lock (_listeners)
                    if (_listeners.Contains(_listener))
                        _listeners.Remove(_listener);
            }

            private readonly IList<TListener> _listeners;
            private readonly TListener _listener;
        }

        #endregion
    }
}
