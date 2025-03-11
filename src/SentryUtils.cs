namespace LgkProductions.Discord.Activity;

static class SentryUtils
{
    public static ISpan StartChild(string operation)
        => SentrySdk.GetSpan()?.StartChild(operation) ?? SentrySdk.StartTransaction(operation, operation);
}
