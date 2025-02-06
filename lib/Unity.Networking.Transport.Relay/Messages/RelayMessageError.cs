using ShortDev.IO.Input;
using ShortDev.IO.Output;

namespace Unity.Networking.Transport.Relay.Messages;

public readonly struct RelayMessageError : IMessage<RelayMessageError>
{
    public required RelayAllocationId AllocationId { get; init; }
    public required RelayErrorCode ErrorCode { get; init; }

    public void Write(ref EndianWriter writer)
    {
        writer.Write(AllocationId);
        writer.Write((byte)ErrorCode);
    }

    public static RelayMessageError Parse(ref EndianReader reader)
    {
        return new()
        {
            AllocationId = reader.Read<RelayAllocationId>(),
            ErrorCode = (RelayErrorCode)reader.ReadByte()
        };
    }
}

public enum RelayErrorCode : byte
{
    InvalidProtocolVersion,
    InactivityTimeout,
    Unauthorized,
    AllocationIdMismatch,
    AllocationIdNotFound,
    NotConnected,
    SelfConnectionNotAllowed
}

public static class RelayErrorCodeExtension
{
    public static string GetDescription(this RelayErrorCode errorCode)
    {
        return errorCode switch
        {
            RelayErrorCode.InvalidProtocolVersion => "Received error message from Relay: invalid protocol version. Make sure your Unity Transport package is up to date.",
            RelayErrorCode.InactivityTimeout => "Received error message from Relay: player timed out due to inactivity.",
            RelayErrorCode.Unauthorized => "Received error message from Relay: unauthorized.",
            RelayErrorCode.AllocationIdMismatch => "Received error message from Relay: allocation ID client mismatch.",
            RelayErrorCode.AllocationIdNotFound => "Received error message from Relay: allocation ID not found.",
            RelayErrorCode.NotConnected => "Received error message from Relay: not connected.",
            RelayErrorCode.SelfConnectionNotAllowed => "Received error message from Relay: self-connect not allowed.",
            _ => $"Received error message from Relay with unknown error code {errorCode}",
        };
    }
}
