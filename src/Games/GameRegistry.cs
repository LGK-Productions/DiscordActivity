using LgkProductions.Discord.Activity.Relay;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ConnectionInfo = LgkProductions.Discord.Activity.Relay.ConnectionInfo;

namespace LgkProductions.Discord.Activity.Games;

public sealed class GameRegistry
{
    readonly ConcurrentDictionary<string, GameInstance> _games = new();
    readonly ConcurrentDictionary<ServerConnectionData, ConnectionInfo> _connectionLookup = [];
    public RelayAllocation CreateOrJoin(RelayAllocationRequest request)
    {
        CreateOrJoinActivity activity = new(request);

        var instance = _games.AddOrUpdate(
            request.InstanceId,
            static (id, activity) =>
            {
                var instance = GameInstance.Create(activity.Request.InstanceId);
                activity.Player = instance.Host;
                return instance;
            },
            static (id, instance, activity) =>
            {
                activity.Player = instance.NewPlayer();
                return instance;
            },
            activity
        );

        Debug.Assert(activity.Player is not null);

        _connectionLookup.TryAdd(new ServerConnectionData(instance.Id, activity.Player.Id), new ConnectionInfo(instance, activity.Player));

        return new(
            AllocationId: activity.Player.Id.Value,
            ConnectionData: new ServerConnectionData(instance.Id, activity.Player.Id).ToBase64String(),
            HostConnectionData: new ServerConnectionData(instance.Id, instance.Host.Id).ToBase64String(),
            Key: Convert.ToBase64String(instance.Key.Span),
            IsHost: activity.Player.Id == instance.Host.Id
        );
    }

    public bool TryGetGame(string instanceId, [MaybeNullWhen(false)] out GameInstance instance)
        => _games.TryGetValue(instanceId, out instance);

    sealed record CreateOrJoinActivity(RelayAllocationRequest Request)
    {
        public GamePlayerInfo? Player { get; set; } = null;
    }

    public bool TryGetConnection(ServerConnectionData connectionData, [MaybeNullWhen(false)] out GameInstance game, [MaybeNullWhen(false)] out GamePlayerInfo player)
    {
        (game, player) = (null, null);

        if (!_connectionLookup.TryGetValue(connectionData, out var entry))
            return false;

        (game, player) = entry;
        return true;
    }
}
