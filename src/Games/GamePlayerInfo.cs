using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using Unity.Networking.Transport.Relay;

namespace LgkProductions.Discord.Activity.Games;

public record GamePlayerInfo(RelayAllocationId Id)
{
    WebSocket? _currentSocket;

    public bool TrySetSocket(WebSocket socket)
        => Interlocked.CompareExchange(ref _currentSocket, socket, null) == null;

    public bool TryGetValidSocket([MaybeNullWhen(false)] out WebSocket socket)
    {
        socket = null;

        var currentSocket = Volatile.Read(ref _currentSocket);
        if (currentSocket is not { State: WebSocketState.Open })
            return false;

        socket = currentSocket;
        return true;
    }

    public async ValueTask DisconnectAsync()
    {
        if (!TryGetValidSocket(out var socket))
            return;

        await socket.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, "Host disconnected", default);
    }
}
