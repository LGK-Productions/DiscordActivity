using ShortDev.IO.Input;
using ShortDev.IO.Output;
using System.Runtime.InteropServices;

namespace Unity.Networking.Transport.Relay.Messages;

[StructLayout(LayoutKind.Sequential)]
public readonly struct RelayHeader : IMessage<RelayHeader>
{
    const ushort DefaultSignature = 0xDA72;
    const ushort DefaultSignatureLittleEndian = 0x72DA;
    const byte CurrentVersion = 0;

    public static int Size { get; } = 4;

    public readonly ushort Signature { get; init; }
    public readonly byte Version { get; init; }
    public readonly RelayMessageType Type { get; init; }

    public bool IsValid => Signature == DefaultSignature && Version == CurrentVersion;

    public void Write(ref EndianWriter writer)
    {
        writer.Write(Signature);
        writer.Write(Version);
        writer.Write((byte)Type);
    }

    public static RelayHeader Create(RelayMessageType type)
    {
        return new RelayHeader
        {
            Signature = DefaultSignature,
            Version = CurrentVersion,
            Type = type,
        };
    }

    public static RelayHeader Parse(ref EndianReader reader)
    {
        return new RelayHeader
        {
            Signature = reader.ReadUInt16(),
            Version = reader.ReadByte(),
            Type = (RelayMessageType)reader.ReadByte(),
        };
    }
}
