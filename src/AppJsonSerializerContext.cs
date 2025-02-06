using LgkProductions.Discord.Activity.Auth;
using LgkProductions.Discord.Activity.Relay;
using System.Text.Json.Serialization;

namespace LgkProductions.Discord.Activity;

[JsonSerializable(typeof(DiscordTokenResponse))]
[JsonSerializable(typeof(DiscordUser))]
[JsonSerializable(typeof(CodeAuthRequest))]
[JsonSerializable(typeof(RelayAllocationRequest))]
[JsonSerializable(typeof(RelayAllocation))]
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
