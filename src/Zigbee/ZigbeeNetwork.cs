using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Lsquared.SmartHome.Zigbee.Extensibility;
using Lsquared.SmartHome.Zigbee.Protocol;
using Lsquared.SmartHome.Zigbee.Protocol.Commands;
using Lsquared.SmartHome.Zigbee.Transports;
using Lsquared.SmartHome.Zigbee.ZDO;

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

            ////_transport = transport; // keep ref to transport
            _protocol = protocol;
            _reader = transport.CreateReader(protocol.PacketExtractor, protocol.PacketEncoder);
            _writer = transport.CreateWriter(protocol.PacketEncoder);

            ////_unsubscriberPayloadListener = Subscribe((IPayloadListener)this);
            //_unsubscriberCommandListener = Subscribe(new CommandPayloadListener(_payloadListeners));
            //_unsubscriberZclCommandListener = Subscribe(new ZclCommandPayloadListener(_payloadListeners));

            var taskScheduler = new LimitedConcurrencyLevelTaskScheduler(2);
            var factory = new TaskFactory(taskScheduler);
            _consumerTask = factory.StartNew(ConsumeAsync);

            //(_consumerThread = new Thread(Consume)).Start();
        }

        public async ValueTask DisposeAsync()
        {
            _stoppingCts.Cancel();
            //_consumerThread.Join(2500);
            //if (_consumerThread.IsAlive)
            //    _consumerThread.Abort();
            await _consumerTask;

            ////_unsubscriberPayloadListener.Dispose();
            ////_unsubscriberCommandListener.Dispose();

            //return default;
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
            Debug.WriteLine("[DEBUG] > " + BitConverter.ToString(packet.ToArray()).Replace("-", " "));
            return _writer.WriteAsync(packet);
        }

        #endregion

        #region SendAndReceive method

        public async Task<ICommandPayload?> SendAndReceiveAsync(ICommandPayload payload, TimeSpan timeout)
        {
            var request = _protocol.CreateRequest(payload);
            if (request is null) return null;
            var receiving = ReceiveAsync(request.ExpectedResponseCode, timeout);
            await SendAsync(request);
            var response = await receiving;
            return response?.Payload;
        }

        #endregion

        #region Subscribe methods

        public IDisposable Subscribe(IPacketListener listener)
        {
            if (!_packetListeners.Contains(listener))
                _packetListeners.Add(listener);
            return new Unsubscriber<IPacketListener>(_packetListeners, listener);
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

        public IDisposable Subscribe(ZDO.IDeviceListener listener)
        {
            if (!_deviceAnnounceListeners.Contains(listener))
                _deviceAnnounceListeners.Add(listener);
            return new Unsubscriber<ZDO.IDeviceListener>(_deviceAnnounceListeners, listener);
        }

        #endregion

        private async Task ConsumeAsync()
        {
            try
            {
                var stoppingToken = _stoppingCts.Token;

                //await _protocol.InitializeAsync(this);

                var packets = _reader.ReadAsync();
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

                    if (command.Payload is ZDO.DeviceAnnounceIndicationPayload p1)
                    {
                        var node = new Node(this, p1.ExtAddr) with { NwkAddr = p1.NwkAddr };
                        _ = _nodes.Add(node);

                        foreach (var listener in _deviceAnnounceListeners)
                            listener.OnNext(node);
                    }
                    else if (command.Payload is ZDO.GetDevicesResponsePayload p2)
                    {
                        foreach (var device in p2.Devices)
                        {
                            var node = new Node(this, device.ExtAddr) with { NwkAddr = device.NwkAddr };
                            _ = _nodes.Add(node);

                            foreach (var listener in _deviceAnnounceListeners)
                                listener.OnNext(node);
                        }
                    }
                    else if (command.Payload is DeviceAnnounceIndicationPayload p3)
                    {
                        var node = new Node(this, p3.ExtAddr) with { NwkAddr = p3.NwkAddr };
                        _ = _nodes.Add(node);
                    }
                    else if (command.Payload is GetDevicesResponsePayload p4)
                    {
                        foreach (var device in p4.Devices)
                        {
                            var node = new Node(this, device.ExtAddr) with { NwkAddr = device.NwkAddr };
                            _ = _nodes.Add(node);
                        }
                    }
                    else if (command.Payload is ZCL.ICommand zclCommand)
                    {
                        foreach (var listener in _zclCommandListeners)
                            listener.OnNext(zclCommand);
                    }

                    if (stoppingToken.IsCancellationRequested)
                        break;
                }
            }
            catch (Exception ex)
            {
                _ = ex;
            }
        }

        #region Fields

        private readonly CancellationTokenSource _stoppingCts = new();

        private readonly List<IPacketListener> _packetListeners = new();
        private readonly List<ICommandListener> _commandListeners = new();
        private readonly List<IPayloadListener> _payloadListeners = new();
        private readonly List<ZDO.IDeviceListener> _deviceAnnounceListeners = new();
        private readonly List<ZCL.ICommandListener> _zclCommandListeners = new();

        private readonly NodeCollection _nodes = new NodeCollection();
        ////private readonly Dictionary<NWK.GroupAddress, string> _groups = new();

        ////private readonly ITransport _transport;
        private readonly IProtocol _protocol;
        private readonly ITransportReader _reader;
        private readonly ITransportWriter _writer;
        //private readonly Thread _consumerThread;
        private readonly Task _consumerTask;

        ////private readonly IDisposable _unsubscriberPayloadListener;
        //private readonly IDisposable _unsubscriberCommandListener;

        private readonly Dictionary<ushort, Func<ZCL.Cluster>> _registeredClusters = new();

        #endregion

        #region Nested types

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

        #endregion
    }
}
