using System.Net;
using System.Net.Sockets;

namespace Ookii.VmSockets;

// This class contains helpers to work around the fact that the SocketAddress class's interface
// is pretty ridiculous pre-.Net 8.0, as it doesn't expose the buffer, just an index operator.
internal static class SocketAddressHelper
{
    private const int GuidSize = 16;

    public static void WriteGuid(this SocketAddress address, int offset, Guid value)
    {
#if NET8_0_OR_GREATER
        value.TryWriteBytes(address.Buffer.Span[offset..]);
#else
        address.Write(offset, value.ToByteArray());
#endif
    }

    public static Guid ReadGuid(this SocketAddress address, int offset)
    {
#if NET8_0_OR_GREATER
        return new Guid(address.Buffer.Span[offset..(offset + GuidSize)]);
#else
        return new Guid(address.Read(offset, GuidSize));
#endif
    }

#if NET8_0_OR_GREATER

    public static AddressFamily GetRealAddressFamily(this SocketAddress address)
        => (AddressFamily)BitConverter.ToUInt16(address.Buffer.Span[0..2]);

#else

    private static void Write(this SocketAddress address, int offset, byte[] value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            address[offset + i] = value[i];
        }
    }

    private static byte[] Read(this SocketAddress address, int offset, int length)
    {
        byte[] buffer = new byte[length];
        for (int i = 0; i < length; i++)
        {
            buffer[i] = address[offset + i];
        }

        return buffer;
    }

#endif
}
