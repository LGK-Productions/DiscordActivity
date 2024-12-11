using System.Text.Json.Serialization;

namespace LgkProductions.Discord.Activity.Auth;

[JsonSerializable(typeof(DiscordTokenResponse))]
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
