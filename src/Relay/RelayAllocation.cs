namespace LgkProductions.Discord.Activity.Relay;

sealed record RelayAllocation(Guid AllocationId, string ConnectionData, string HostConnectionData, string Key, bool IsHost);
