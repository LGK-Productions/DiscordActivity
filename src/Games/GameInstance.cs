using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Unity.Networking.Transport.Relay;

namespace LgkProductions.Discord.Activity.Games;

public record GameInstance(string InstanceId, GamePlayerInfo Host) : IEnumerable<GamePlayerInfo>
{
    public Guid Id { get; } = Guid.NewGuid();
    public ReadOnlyMemory<byte> Key { get; } = RandomNumberGenerator.GetBytes(64);

    readonly ConcurrentBag<GamePlayerInfo> _players = [];

    public bool TryGetPlayer(RelayAllocationId playerId, [MaybeNullWhen(false)] out GamePlayerInfo player)
    {
        player = _players.SingleOrDefault(x => x.Id == playerId);
        return player != null;
    }

    public GamePlayerInfo NewPlayer()
    {
        GamePlayerInfo info = new(RelayAllocationId.Create());
        _players.Add(info);
        return info;
    }

    public IEnumerator<GamePlayerInfo> GetEnumerator()
        => _players.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _players.GetEnumerator();

    public static GameInstance Create(string instanceId)
    {
        GameInstance instance = new(instanceId, Host: new GamePlayerInfo(RelayAllocationId.Create()));
        instance._players.Add(instance.Host);
        return instance;
    }
}
