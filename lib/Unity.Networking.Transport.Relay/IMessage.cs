using ShortDev.IO.Input;
using ShortDev.IO.Output;

namespace Unity.Networking.Transport.Relay;

public interface IMessage<TSelf> where TSelf : IMessage<TSelf>
{
    void Write(ref EndianWriter writer);
    static abstract TSelf Parse(ref EndianReader reader);
}

public static class MessageExtensions
{
    public static void Write<T>(this ref EndianWriter writer, in IMessage<T> message) where T : IMessage<T>
        => message.Write(ref writer);

    public static T Read<T>(this ref EndianReader reader) where T : IMessage<T>
        => T.Parse(ref reader);
}
