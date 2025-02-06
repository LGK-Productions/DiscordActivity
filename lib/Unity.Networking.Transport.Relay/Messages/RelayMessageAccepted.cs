using ShortDev.IO.Input;
using ShortDev.IO.Output;

namespace Unity.Networking.Transport.Relay.Messages;

public readonly struct RelayMessageAccepted : IMessage<RelayMessageAccepted>
{
    public required RelayMessageFromTo Header { get; init; }

    public void Write(ref EndianWriter writer)
        => writer.Write(Header);

    public static RelayMessageAccepted Parse(ref EndianReader reader)
    {
        return new()
        {
            Header = reader.Read<RelayMessageFromTo>()
        };
    }
}
