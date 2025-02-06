namespace LgkProductions.Discord.Activity.Relay;

public sealed record RelayAllocation(Guid AllocationId, string ConnectionData, string HostConnectionData, string Key, bool IsHost);
