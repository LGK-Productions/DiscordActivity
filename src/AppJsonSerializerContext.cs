using LgkProductions.Discord.Activity.Auth;
using System.Text.Json.Serialization;

namespace LgkProductions.Discord.Activity;

[JsonSerializable(typeof(DiscordTokenResponse))]
[JsonSerializable(typeof(DiscordUser))]
[JsonSerializable(typeof(CodeAuthRequest))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
    public static void Register(WebApplicationBuilder builder)
    {
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });
    }
}
