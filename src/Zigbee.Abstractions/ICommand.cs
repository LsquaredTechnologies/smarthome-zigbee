namespace Lsquared.SmartHome.Zigbee
{
    public interface ICommand
    {
        ICommandHeader Header { get; }

        ICommandPayload Payload { get; }

        ////int WriteTo(ref Span<byte> span, int offset);
    }
}
