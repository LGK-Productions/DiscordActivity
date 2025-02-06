using ShortDev.IO.Input;
using ShortDev.IO.Output;
using System.Runtime.CompilerServices;

namespace Unity.Networking.Transport.Relay;

[InlineArray(32)]
public struct HMAC : IMessage<HMAC>
{
    public byte _element0;

    public readonly void Write(ref EndianWriter writer)
        => writer.Write(this);

    public static HMAC Parse(ref EndianReader reader)
    {
        HMAC result = default;
        reader.ReadBytes(result);
        return result;
    }
}
