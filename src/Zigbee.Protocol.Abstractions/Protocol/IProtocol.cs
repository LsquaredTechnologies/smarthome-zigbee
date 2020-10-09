using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Lsquared.SmartHome.Zigbee.Protocol.Raw;

namespace Lsquared.SmartHome.Zigbee.Protocol
{
    public interface IProtocol
    {
        IPacketExtractor PacketExtractor { get; }

        IPacketEncoder PacketEncoder { get; }

        Request? CreateRequest(ICommandPayload payload);

        [return: NotNull]
        Func<ICommand, bool> ExpectResponseCode(ushort responseCode);

        ValueTask InitializeAsync(INetwork network);

        [return: NotNull]
        ICommand Read(ReadOnlyMemory<byte> memory);

        int Write(Request request, ref Span<byte> span);

        int Write(Response response, ref Span<byte> span);
    }
}
