using ShortDev.IO.Input;
using ShortDev.IO.Output;
using System.Buffers;

namespace Unity.Networking.Transport.Relay.Messages;

public readonly struct RelayMessageBind : IMessage<RelayMessageBind>, IDisposable
{
    public readonly byte AcceptMode { get; init; }
    public readonly ushort Nonce { get; init; }
    public ReadOnlyMemory<byte> ConnectionData { get; init; }
    public HMAC Hmac { get; init; }

    public void Write(ref EndianWriter writer)
    {
        writer.Write(AcceptMode);
        writer.Write(Nonce);
        writer.Write((byte)ConnectionData.Length);
        writer.Write(ConnectionData.Span);
        writer.Write<HMAC>(Hmac);
    }

    public void Dispose()
        => ConnectionData.TryReturn(ArrayPool<byte>.Shared);

    public static RelayMessageBind Parse(ref EndianReader reader)
    {
        var acceptMode = reader.ReadByte();
        var nonce = reader.ReadUInt16();

        var connectDataSize = reader.ReadByte();
        var buffer = ArrayPool<byte>.Shared.Rent(connectDataSize);
        reader.ReadBytes(buffer.AsSpan(0, connectDataSize));

        return new RelayMessageBind
        {
            AcceptMode = acceptMode,
            Nonce = nonce,
            ConnectionData = buffer.AsMemory(0, connectDataSize),
            Hmac = reader.Read<HMAC>()
        };
    }
}
