using System.Text.Json.Serialization;

namespace LgkProductions.Discord.Activity.Auth;

public sealed record DiscordUser(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("username")] string UserName,
    [property: JsonPropertyName("avatar")] string Avatar
);
