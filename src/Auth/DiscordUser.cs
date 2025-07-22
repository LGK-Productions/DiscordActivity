using System.Text.Json.Serialization;

namespace LgkProductions.Discord.Activity.Auth;

sealed record DiscordUser(
   [property: JsonPropertyName("id")] string Id,
   [property: JsonPropertyName("username")] string UserName,
   [property: JsonPropertyName("avatar")] string Avatar
);
