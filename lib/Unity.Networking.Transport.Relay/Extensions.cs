using System.Buffers;
using System.Runtime.InteropServices;

namespace Unity.Networking.Transport.Relay;

internal static class Extensions
{
    public static bool TryReturn<T>(this ReadOnlyMemory<T> source, ArrayPool<T> pool, bool clearArray = false)
    {
        if (!MemoryMarshal.TryGetArray(source, out var segment))
            return false;

        if (segment.Array is null)
            return false;

        pool.Return(segment.Array, clearArray);
        return true;
    }
}
