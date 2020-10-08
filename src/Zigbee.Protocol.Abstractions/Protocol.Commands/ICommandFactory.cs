using System;
using System.Diagnostics.CodeAnalysis;

namespace Lsquared.SmartHome.Zigbee.Protocol.Commands
{
    public interface ICommandFactory
    {
        ICommand Create(ReadOnlySpan<byte> packetSpan);
    }
}
