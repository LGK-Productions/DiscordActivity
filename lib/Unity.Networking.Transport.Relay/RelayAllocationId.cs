using ShortDev.IO.Input;
using ShortDev.IO.Output;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Unity.Networking.Transport.Relay;

[JsonConverter(typeof(RelayAllocationIdConverter))]
public readonly record struct RelayAllocationId(Guid Value) : IMessage<RelayAllocationId>
{
    public readonly void Write(ref EndianWriter writer)
    {
        Span<byte> buffer = stackalloc byte[16];
        if (!Value.TryWriteBytes(buffer))
            Debug.Fail("Could not write Guid");

        writer.Write(buffer);
    }

    public override string ToString()
        => Value.ToString();

    public static RelayAllocationId Parse(ref EndianReader reader)
    {
        Span<byte> buffer = stackalloc byte[16];
        reader.ReadBytes(buffer);
        return new(new Guid(buffer));
    }

    public static RelayAllocationId Create()
        => new(Guid.NewGuid());
}

public sealed class RelayAllocationIdConverter : JsonConverter<RelayAllocationId>
{
    public override RelayAllocationId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetGuid());

    public override void Write(Utf8JsonWriter writer, RelayAllocationId value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value.ToString());
}
