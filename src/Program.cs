using LgkProductions.Discord.Activity;
using LgkProductions.Discord.Activity.Auth;
using LgkProductions.Discord.Activity.Games;
using LgkProductions.Discord.Activity.Relay;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateSlimBuilder(args);

AppJsonSerializerContext.Register(builder);

builder.Services.AddCors();
builder.Services.AddHttpClient();

builder.Services.AddHttpClient<DiscordClient>();
builder.Services.AddSingleton<GameRegistry>();
builder.Services.AddSingleton<RelayManager>();

var app = builder.Build();
app.UseCors(builder =>
{
    builder.WithOrigins(
        app.Configuration.GetValue<string[]>("CorsOrigins") ?? []
    );
});
app.UseWebSockets();

app.Map(
    "/",
    async ([FromServices] GameRegistry reg, [FromServices] RelayManager relayManager, HttpContext ctx, CancellationToken cancellation) =>
    {
        if (!ctx.WebSockets.IsWebSocketRequest)
        {
            ctx.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        var socket = await ctx.WebSockets.AcceptWebSocketAsync();

        await relayManager.RunSocketLoopAsync(socket, cancellation);
    }
);

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

app.MapPost(
    "/relay/allocate",
    ([FromBody] RelayAllocationRequest request, [FromServices] GameRegistry reg) =>
    {
        return reg.CreateOrJoin(request);
    }
);

app.Run();
