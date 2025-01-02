using LgkProductions.Discord.Activity;
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

app.MapPost("/token", async ([FromBody] CodeAuthRequest request, [FromServices] DiscordClient client) =>
{
    var token = await client.RequestAccessTokenAsync(request.Code);
    if (token is null)
        return Results.BadRequest("Invalid code");

    return Results.Ok(token);
});

app.MapPost("/token/user", async ([FromBody] CodeAuthRequest request, [FromServices] DiscordClient client) =>
{
    var token = await client.RequestAccessTokenAsync(request.Code);
    if (token is null)
        return Results.BadRequest("Invalid code");

    var user = await client.RequestUserAsync(token);
    if (token is null)
        return Results.BadRequest("Invalid token");

    return Results.Ok(user);
});

app.Run();
