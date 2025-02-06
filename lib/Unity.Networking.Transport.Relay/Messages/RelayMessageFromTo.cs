using ShortDev.IO.Input;
using ShortDev.IO.Output;

namespace Unity.Networking.Transport.Relay.Messages;

public readonly struct RelayMessageFromTo : IMessage<RelayMessageFromTo>
{
    public readonly RelayAllocationId FromAllocationId { get; init; }
    public readonly RelayAllocationId ToAllocationId { get; init; }

    public void Write(ref EndianWriter writer)
    {
        writer.Write(FromAllocationId);
        writer.Write(ToAllocationId);
    }

    public static RelayMessageFromTo Parse(ref EndianReader reader)
    {
        return new()
        {
            FromAllocationId = reader.Read<RelayAllocationId>(),
            ToAllocationId = reader.Read<RelayAllocationId>()
        };
    }
}
