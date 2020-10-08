using System;
using System.Threading;
using System.Threading.Tasks;
using Lsquared.SmartHome.Zigbee.Protocol.Commands;

namespace Lsquared.SmartHome.Zigbee
{
    internal sealed class ReceiveCommandListener : ICommandListener
    {
        public TaskCompletionSource<ICommand> Result { get; private set; }

        public ReceiveCommandListener(TimeSpan timeout, Func<ICommand, bool> predicate)
        {
            _predicate = predicate;
            Result = new TaskCompletionSource<ICommand>();
            _cts = new(timeout);
            _cts.Token.Register(() =>
            {
                if (!Result.Task.IsCompleted)
                    Result.TrySetCanceled();
            });
        }

        void ICommandListener.OnNext(ICommand command)
        {
            if (_predicate(command))
                Result.TrySetResult(command);
        }

        private readonly Func<ICommand, bool> _predicate;
        private readonly CancellationTokenSource _cts;
    }
}
