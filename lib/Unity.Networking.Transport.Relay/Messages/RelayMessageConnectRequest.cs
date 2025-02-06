using ShortDev.IO.Input;
using ShortDev.IO.Output;
using System.Buffers;

namespace Unity.Networking.Transport.Relay.Messages;

public readonly struct RelayMessageConnectRequest : IMessage<RelayMessageConnectRequest>, IDisposable
{
    public required RelayAllocationId AllocationId { get; init; }
    public required ReadOnlyMemory<byte> ToConnectionData { get; init; }

    public void Write(ref EndianWriter writer)
    {
        writer.Write(AllocationId);
        writer.Write((byte)ToConnectionData.Length);
        writer.Write(ToConnectionData.Span);
    }

    public void Dispose()
        => ToConnectionData.TryReturn(ArrayPool<byte>.Shared);

    public static RelayMessageConnectRequest Parse(ref EndianReader reader)
    {
        var allocationId = reader.Read<RelayAllocationId>();

        var connectDataSize = reader.ReadByte();
        var buffer = ArrayPool<byte>.Shared.Rent(connectDataSize);
        reader.ReadBytes(buffer.AsSpan(0, connectDataSize));

        return new RelayMessageConnectRequest
        {
            AllocationId = allocationId,
            ToConnectionData = buffer.AsMemory(0, connectDataSize)
        };
    }
}
