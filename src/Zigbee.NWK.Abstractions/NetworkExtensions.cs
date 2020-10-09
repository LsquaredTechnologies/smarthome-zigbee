using System;
using System.Threading.Tasks;

namespace Lsquared.SmartHome.Zigbee
{
    public static class NetworkExtensions
    {
        public static Task<ICommand?> ReceiveAsync(this INetwork network, ushort responseCode) =>
            network.ReceiveAsync(responseCode, TimeSpan.FromMilliseconds(500));

        public static Task<ICommand?> ReceiveAsync(this INetwork network, ushort responseCode, double timeoutInSeconds) =>
            network.ReceiveAsync(responseCode, TimeSpan.FromSeconds(timeoutInSeconds));

        public static Task<ICommand?> ReceiveAsync(this INetwork network, ushort responseCode, int timeoutInMilliseconds) =>
            network.ReceiveAsync(responseCode, TimeSpan.FromMilliseconds(timeoutInMilliseconds));

        public static Task<TCommandPayload> SendAndReceiveAsync<TCommandPayload>(this INetwork network, ICommandPayload payload)
            where TCommandPayload : ICommandPayload =>
            network.SendAndReceiveAsync<TCommandPayload>(payload, TimeSpan.FromMilliseconds(500));

        public static Task<TCommandPayload> SendAndReceiveAsync<TCommandPayload>(this INetwork network, ICommandPayload payload, int timeoutInMilliseconds)
            where TCommandPayload : ICommandPayload =>
            network.SendAndReceiveAsync<TCommandPayload>(payload, TimeSpan.FromMilliseconds(timeoutInMilliseconds));

        public static Task<TCommandPayload> SendAndReceiveAsync<TCommandPayload>(this INetwork network, ICommandPayload payload, double timeoutInSeconds)
            where TCommandPayload : ICommandPayload =>
            network.SendAndReceiveAsync<TCommandPayload>(payload, TimeSpan.FromSeconds(timeoutInSeconds));

        public static async Task<TCommandPayload> SendAndReceiveAsync<TCommandPayload>(this INetwork network, ICommandPayload payload, TimeSpan timeout)
            where TCommandPayload : ICommandPayload
        {
            var responsePayload = await network.SendAndReceiveAsync(payload, timeout);
            return responsePayload is TCommandPayload p ? p : default!;
        }

        public static Task<ICommandPayload?> SendAndReceiveAsync(this INetwork network, ICommandPayload payload) =>
            network.SendAndReceiveAsync(payload, TimeSpan.FromMilliseconds(500));

        public static Task<ICommandPayload?> SendAndReceiveAsync(this INetwork network, ICommandPayload payload, int timeoutInMilliseconds) =>
            network.SendAndReceiveAsync(payload, TimeSpan.FromMilliseconds(timeoutInMilliseconds));

        public static Task<ICommandPayload?> SendAndReceiveAsync(this INetwork network, ICommandPayload payload, double timeoutInSeconds) =>
            network.SendAndReceiveAsync(payload, TimeSpan.FromSeconds(timeoutInSeconds));
    }
}
