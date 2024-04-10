using System.Net;
using System.Net.Sockets;

namespace Ookii.VmSockets.Tests;

static class SocketAddressHelper
{
    public static Memory<byte> GetBuffer(this SocketAddress address)
    {
#if NET8_0_OR_GREATER
        return address.Buffer;
#else
        byte[] buffer = new byte[address.Size];
        for (int i = 0; i < address.Size; i++)
        {
            buffer[i] = address[i];
        }

        return buffer.AsMemory();
#endif
    }
}
