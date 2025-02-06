using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Unity.Networking.Transport.Relay;

namespace LgkProductions.Discord.Activity.Relay;

public readonly record struct ServerConnectionData(Guid GameId, RelayAllocationId AllocationId)
{
    public string ToBase64String()
    {
        Span<byte> buffer = stackalloc byte[16 + 16];
        if (!GameId.TryWriteBytes(buffer))
            throw new UnreachableException($"Could not write {nameof(GameId)}");

        if (!AllocationId.Value.TryWriteBytes(buffer[16..]))
            throw new UnreachableException($"Could not write {nameof(GameId)}");

        return Convert.ToBase64String(buffer);
    }

    public static bool TryParse(ReadOnlySpan<byte> buffer, [MaybeNullWhen(false)] out ServerConnectionData connectionData)
    {
        connectionData = default;

        if (buffer.Length < 16 + 16)
            return false;

        var instanceId = new Guid(buffer[..16]);
        var allocationId = new RelayAllocationId(new Guid(buffer[16..32]));
        connectionData = new(instanceId, allocationId);
        return true;
    }
}
