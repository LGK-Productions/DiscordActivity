using ShortDev.IO.Input;
using ShortDev.IO.Output;
using System.Buffers;

namespace Unity.Networking.Transport.Relay.Messages;

public readonly struct RelayMessageRelay : IMessage<RelayMessageRelay>, IDisposable
{
    public required RelayMessageFromTo Header { get; init; }
    public required ReadOnlyMemory<byte> Payload { get; init; }

    public void Write(ref EndianWriter writer)
    {
        writer.Write(Header);
        writer.WriteWithLength(Payload.Span);
    }

    public void Dispose()
        => Payload.TryReturn(ArrayPool<byte>.Shared);

    public static RelayMessageRelay Parse(ref EndianReader reader)
    {
        var header = reader.Read<RelayMessageFromTo>();

        var payloadSize = reader.ReadUInt16();
        var buffer = ArrayPool<byte>.Shared.Rent(payloadSize);
        reader.ReadBytes(buffer.AsSpan(0, payloadSize));

        return new RelayMessageRelay
        {
            Header = header,
            Payload = buffer.AsMemory(0, payloadSize)
        };
    }
}
