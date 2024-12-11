using System.Text.Json.Serialization;

namespace LgkProductions.Discord.Activity.Auth;

public record DiscordTokenResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }
}
