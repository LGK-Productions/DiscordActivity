namespace LgkProductions.Discord.Activity.Auth;

sealed class DiscordClient(HttpClient client, IConfiguration config)
{
    readonly HttpClient _client = client;
    readonly IConfiguration _config = config;

    public async Task<string?> RequestAccessTokenAsync(string code)
    {
        using FormUrlEncodedContent content = new(new Dictionary<string, string?>()
        {
            { "client_id", _config["DiscordClientId"] },
            { "client_secret", _config["DiscordClientSecret"] },
            { "grant_type", "authorization_code" },
            { "code", code }
        });

        using var response = await _client.PostAsync(DiscordTokenEndpoint, content).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            return null;

        var tokenResponse = await response.Content.ReadFromJsonAsync(AppJsonSerializerContext.Default.DiscordTokenResponse).ConfigureAwait(false);
        return tokenResponse?.AccessToken;
    }

    public async Task<DiscordUser?> RequestUserAsync(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        using HttpRequestMessage message = new(HttpMethod.Get, "https://discord.com/api/users/@me")
        {
            Headers =
            {
                Authorization = new("Bearer", token)
            }
        };
        using var response = await _client.SendAsync(message).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync(AppJsonSerializerContext.Default.DiscordUser).ConfigureAwait(false);
    }

    static readonly Uri DiscordTokenEndpoint = new("https://discord.com/api/oauth2/token");
}
