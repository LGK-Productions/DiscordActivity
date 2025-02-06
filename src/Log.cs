using LgkProductions.Discord.Activity.Relay;
using System.Net.WebSockets;
using Unity.Networking.Transport.Relay;
using Unity.Networking.Transport.Relay.Messages;

namespace LgkProductions.Discord.Activity;

public static partial class Log
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "New socket {socket}")]
    public static partial void NewSocket(this ILogger<RelayManager> logger, WebSocket socket);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Received relay message '{messageType}'")]
    public static partial void ReceivedMessage(this ILogger<RelayManager> logger, RelayMessageType messageType);

    [LoggerMessage(Level = LogLevel.Information, Message = "Sending error {error} to socket {socket}: {description}")]
    public static partial void SendingError(this ILogger<RelayManager> logger, WebSocket socket, RelayErrorCode error, string description);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Exception in relay loop for {socket}")]
    public static partial void ExceptionInRelayLoop(this ILogger<RelayManager> logger, Exception ex, WebSocket socket);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Socket {socket} disconnected")]
    public static partial void SocketDisconnected(this ILogger<RelayManager> logger, WebSocket socket);
}
