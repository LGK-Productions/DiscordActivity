using ShortDev.IO.Input;
using ShortDev.IO.Output;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Unity.Networking.Transport.Relay;

[InlineArray(32)]
public struct HMAC : IMessage<HMAC>, IEquatable<HMAC>
{
    byte _element0;

    public readonly void Write(ref EndianWriter writer)
        => writer.Write(this);

    public static HMAC Parse(ref EndianReader reader)
    {
        HMAC result = default;
        reader.ReadBytes(result);
        return result;
    }

    public override readonly int GetHashCode()
        => ((ReadOnlySpan<byte>)this).GetHashCode();

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not HMAC other)
            return false;
        return Equals(other);
    }

    public readonly bool Equals(HMAC other)
        => (ReadOnlySpan<byte>)this == (ReadOnlySpan<byte>)other;

    public static bool operator ==(HMAC left, HMAC right)
        => left.Equals(right);

    public static bool operator !=(HMAC left, HMAC right)
        => !left.Equals(right);
}
