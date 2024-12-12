using LgkProductions.Discord.Activity.Auth;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateSlimBuilder(args);
AppJsonSerializerContext.Register(builder);
builder.Services.AddCors();
builder.Services.AddHttpClient();

var app = builder.Build();
app.UseCors(builder =>
{
    builder.WithOrigins(
        app.Configuration.GetValue<string[]>("CorsOrigins") ?? []
    );
});

app.MapPost("/token", async ([FromBody] CodeAuthRequest request, [FromServices] IConfiguration config, [FromServices] IHttpClientFactory httpClientFactory) =>
{
    FormUrlEncodedContent content = new(new Dictionary<string, string?>()
    {
        { "client_id", config["DiscordClientId"] },
        { "client_secret", config["DiscordClientSecret"] },
        { "grant_type", "authorization_code" },
        { "code", request.Code }
    });

    var client = httpClientFactory.CreateClient();
    using var response = await client.PostAsync("https://discord.com/api/oauth2/token", content);
    if (!response.IsSuccessStatusCode)
        return Results.StatusCode((int)response.StatusCode);

    var tokenResponse = await response.Content.ReadFromJsonAsync(AppJsonSerializerContext.Default.DiscordTokenResponse);
    if (tokenResponse is null)
        return Results.BadRequest();

    return Results.Ok(tokenResponse.AccessToken);
});

app.Run();
