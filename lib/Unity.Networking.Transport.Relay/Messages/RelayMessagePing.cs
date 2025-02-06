using ShortDev.IO.Input;
using ShortDev.IO.Output;

namespace Unity.Networking.Transport.Relay.Messages;

public readonly struct RelayMessagePing : IMessage<RelayMessagePing>
{
    public required RelayAllocationId FromAllocationId { get; init; }
    public required ushort SequenceNumber { get; init; }

    public void Write(ref EndianWriter writer)
    {
        writer.Write(FromAllocationId);
        writer.Write(SequenceNumber);
    }

    public static RelayMessagePing Parse(ref EndianReader reader)
    {
        return new()
        {
            FromAllocationId = reader.Read<RelayAllocationId>(),
            SequenceNumber = reader.ReadUInt16()
        };
    }
}
