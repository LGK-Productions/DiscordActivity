using LgkProductions.Discord.Activity.Games;
using ShortDev.IO.Input;
using ShortDev.IO.Output;
using System.Buffers;
using System.Diagnostics;
using System.Net.WebSockets;
using Unity.Networking.Transport.Relay;
using Unity.Networking.Transport.Relay.Messages;

namespace LgkProductions.Discord.Activity.Relay;

public sealed class RelayManager(ILogger<RelayManager> logger, GameRegistry registry)
{
    readonly ILogger<RelayManager> _logger = logger;
    readonly GameRegistry _registry = registry;

    public async Task RunSocketLoopAsync(WebSocket socket, CancellationToken cancellation)
    {
        _logger.NewSocket(socket);

        ConnectionInfo? connection = null;
        try
        {
            while (socket.State == WebSocketState.Open && !cancellation.IsCancellationRequested)
            {
                var buffer = await socket.ReceiveAsync();
                EndianReader reader = new(ShortDev.IO.Endianness.BigEndian, buffer.Span);

                var header = reader.Read<RelayHeader>();
                _logger.ReceivedMessage(header.Type);
                switch (header.Type)
                {
                    case RelayMessageType.Bind:
                        using (var bindRequest = reader.Read<RelayMessageBind>())
                        {
                            connection = await OnBind(socket, bindRequest, cancellation);
                        }
                        break;

                    case RelayMessageType.ConnectRequest when connection is not null:
                        using (var connectRequest = reader.Read<RelayMessageConnectRequest>())
                        {
                            await OnConnectRequest(socket, connection.Value, connectRequest, cancellation);
                        }
                        break;

                    case RelayMessageType.Relay when connection is not null:
                        using (var relayMessage = reader.Read<RelayMessageRelay>())
                        {
                            await OnRelay(socket, connection.Value, relayMessage, cancellation);
                        }
                        break;

                    case RelayMessageType.Ping:
                        var pingMessage = reader.Read<RelayMessagePing>();
                        break;

                    case RelayMessageType.Disconnect when connection is not null:
                        var disconnectMessage = reader.Read<RelayMessageFromTo>();
                        await OnDisconnect(socket, connection.Value, disconnectMessage, cancellation);
                        break;

                    default:
                        Debugger.Break();
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.ExceptionInRelayLoop(ex, socket);
        }
        finally
        {
            _logger.SocketDisconnected(socket);
            OnConnectionTerminated(connection);
        }
    }

    async ValueTask<ConnectionInfo?> OnBind(WebSocket socket, RelayMessageBind message, CancellationToken cancellation)
    {
        if (!ServerConnectionData.TryParse(message.ConnectionData.Span, out var connectData))
        {
            await SendError(socket, default, RelayErrorCode.NotConnected, cancellation);
            return null;
        }

        // ToDo: Authenticate

        if (!_registry.TryGetConnection(connectData, out var instance, out var player))
        {
            await SendError(socket, default, RelayErrorCode.NotConnected, cancellation);
            return null;
        }

        if (!player.TrySetSocket(socket))
        {
            // ToDo: Do we allow multiple binds?
        }

        EndianWriter writer = new(ShortDev.IO.Endianness.BigEndian);
        writer.Write(RelayHeader.Create(RelayMessageType.BindReceived));

        await socket.SendAsync(writer.Buffer.AsMemory(), WebSocketMessageType.Binary, endOfMessage: true, cancellation);

        return new ConnectionInfo(instance, player);
    }

    async ValueTask OnConnectRequest(WebSocket socket, ConnectionInfo connection, RelayMessageConnectRequest connectRequest, CancellationToken cancellation)
    {
        if (connection.Player.Id != connectRequest.AllocationId)
            throw new UnreachableException("Relay message from wrong player");

        if (!ServerConnectionData.TryParse(connectRequest.ToConnectionData.Span, out var connectData))
        {
            await SendError(socket, connection.Player.Id, RelayErrorCode.AllocationIdNotFound, cancellation);
            return;
        }

        if (!_registry.TryGetConnection(connectData, out var targetInstance, out var targetPlayer))
        {
            await SendError(socket, connection.Player.Id, RelayErrorCode.AllocationIdNotFound, cancellation);
            return;
        }

        if (targetInstance != connection.Game)
        {
            await SendError(socket, connection.Player.Id, RelayErrorCode.Unauthorized, cancellation);
            return;
        }

        EndianWriter writer = new(ShortDev.IO.Endianness.BigEndian);
        writer.Write(RelayHeader.Create(RelayMessageType.Accepted));
        writer.Write(new RelayMessageFromTo()
        {
            FromAllocationId = targetPlayer.Id,
            ToAllocationId = connectRequest.AllocationId
        });

        await socket.SendAsync(writer.Buffer.AsMemory(), WebSocketMessageType.Binary, endOfMessage: true, cancellation);
    }

    async ValueTask OnRelay(WebSocket socket, ConnectionInfo connection, RelayMessageRelay message, CancellationToken cancellation)
    {
        if (connection.Player.Id != message.Header.FromAllocationId)
            throw new UnreachableException("Relay message from wrong player");

        if (!connection.Game.TryGetPlayer(message.Header.ToAllocationId, out var targetPlayer))
        {
            await SendError(socket, connection.Player.Id, RelayErrorCode.AllocationIdNotFound, cancellation);
            return;
        }

        if (!targetPlayer.TryGetValidSocket(out var targetSocket))
        {
            await SendError(socket, connection.Player.Id, RelayErrorCode.NotConnected, cancellation);
            return;
        }

        EndianWriter writer = new(ShortDev.IO.Endianness.BigEndian);
        writer.Write(RelayHeader.Create(RelayMessageType.Relay));
        writer.Write(new RelayMessageRelay()
        {
            Header = new()
            {
                FromAllocationId = connection.Player.Id,
                ToAllocationId = targetPlayer.Id
            },
            Payload = message.Payload
        });

        await targetSocket.SendAsync(writer.Buffer.AsMemory(), WebSocketMessageType.Binary, endOfMessage: true, cancellation);
    }

    static async ValueTask OnDisconnect(WebSocket socket, ConnectionInfo connection, RelayMessageFromTo message, CancellationToken cancellation)
    {
        if (connection.Player.Id != message.FromAllocationId)
            throw new UnreachableException("Relay message from wrong player");

        EndianWriter writer = new(ShortDev.IO.Endianness.BigEndian);
        writer.Write(RelayHeader.Create(RelayMessageType.Disconnect));
        writer.Write(message);

        await socket.SendAsync(writer.Buffer.AsMemory(), WebSocketMessageType.Binary, endOfMessage: true, cancellation);
    }

    void OnConnectionTerminated(ConnectionInfo? connection)
    {
        if (connection is null)
            return;

        var (game, player) = connection.Value;
        if (game.Host.Id != player.Id)
            return;

        _registry.Cleanup(game);

        // ToDo: Notify peers
    }

    async ValueTask SendError(WebSocket socket, RelayAllocationId allocationId, RelayErrorCode error, CancellationToken cancellation)
    {
        EndianWriter writer = new(ShortDev.IO.Endianness.BigEndian);
        writer.Write(RelayHeader.Create(RelayMessageType.Error));
        writer.Write(new RelayMessageError()
        {
            AllocationId = allocationId,
            ErrorCode = error
        });

        _logger.SendingError(socket, error, error.GetDescription());

        await socket.SendAsync(writer.Buffer.AsMemory(), WebSocketMessageType.Binary, endOfMessage: true, cancellation);
    }
}
