using ShortDev.IO.Input;
using ShortDev.IO.Output;
using System.Buffers;
using System.Net.WebSockets;
using Unity.Networking.Transport.Relay.Messages;

namespace LgkProductions.Discord.Activity.Relay;

internal static class WebSocketUtils
{
    static readonly ArrayPool<byte> HeaderPool = ArrayPool<byte>.Create();
    public static async ValueTask<RelayHeader> ReceiveHeaderAsync(this WebSocket socket, CancellationToken cancellation = default)
    {
        byte[] buffer = HeaderPool.Rent(RelayHeader.Size);
        try
        {
            var result = await socket.ReceiveAsync((Memory<byte>)buffer, cancellation);

            EndianReader reader = new(ShortDev.IO.Endianness.BigEndian, buffer);
            return RelayHeader.Parse(ref reader);
        }
        finally
        {
            HeaderPool.Return(buffer);
        }
    }

    public static async ValueTask SendHeaderAsync(this WebSocket socket, RelayHeader header, bool endOfMessage, CancellationToken cancellation = default)
    {
        EndianWriter writer = new(ShortDev.IO.Endianness.BigEndian, RelayHeader.Size);
        header.Write(ref writer);

        await socket.SendAsync(
            writer.Buffer.AsMemory(), WebSocketMessageType.Binary,
            endOfMessage,
            cancellation
        );
    }

    const int WebSocketChunkSize = 1024;

    public static async Task<ReadOnlyMemory<byte>> ReceiveAsync(this WebSocket socket)
    {
        // ToDo: A lot of GC preassure here, consider using a pool

        ArrayBufferWriter<byte> writer = new();

        ValueWebSocketReceiveResult result;
        do
        {
            result = await socket.ReceiveAsync(writer.GetMemory(WebSocketChunkSize), CancellationToken.None);
            writer.Advance(result.Count);
        } while (!result.EndOfMessage);

        return writer.WrittenMemory;
    }
}
