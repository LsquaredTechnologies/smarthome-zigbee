using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lsquared.SmartHome.Zigbee
{
    public interface INetwork : IAsyncDisposable
    {
        INodeCollection Nodes { get; }

        Task<ICommand?> ReceiveAsync(ushort responseCode, TimeSpan timeout);

        ValueTask SendAsync(ICommandPayload payload);

        ValueTask SendAsync(Request request);

        ValueTask SendAsync(ReadOnlyMemory<byte> packet);

        Task<ICommandPayload?> SendAndReceiveAsync(ICommandPayload payload, TimeSpan timeout);
    }
}
