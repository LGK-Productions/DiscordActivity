using Unity.Networking.Transport.Relay.Messages;

namespace Unity.Networking.Transport.Relay;

public enum RelayMessageType : byte
{
    /// <summary>
    /// <see cref="RelayMessageBind"/>
    /// </summary>
    Bind = 0,
    BindReceived = 1,
    Ping = 2,
    /// <summary>
    /// <see cref="RelayMessageConnectRequest"/>
    /// </summary>
    ConnectRequest = 3,
    /// <summary>
    /// <see cref="RelayMessageAccepted"/>
    /// </summary>
    Accepted = 6,
    /// <summary>
    /// <see cref="RelayMessageFromTo"/>
    /// </summary>
    Disconnect = 9,
    Relay = 10,
    Error = 12,
}
