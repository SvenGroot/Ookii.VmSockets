#if NET8_0_OR_GREATER

using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Ookii.VmSockets;

/// <summary>
/// Provides constants and helper methods for working with Linux VSock sockets.
/// </summary>
/// <remarks>
/// <note type="important">
///   Due to limitations in the Sockets API on the Linux version of .Net, this class is only
///   available for .Net 8.0 and later.
/// </note>
/// </remarks>
public static partial class VSock
{
    /// <summary>
    /// The address family for VSock sockets, also known as <c>AF_VSOCK</c>.
    /// </summary>
    public const AddressFamily AddressFamily = (AddressFamily)40;

    /// <summary>
    /// The size of a <c>sockaddr_vm</c> structure.
    /// </summary>
    public const int SocketAddressSize = 16;

    /// <summary>
    /// Wildcard CID or port for VSock sockets.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This value can be used as the CID or port in a <see cref="VSockEndPoint"/> to bind to
    ///   any CID or any available port.
    /// </para>
    /// </remarks>
    public const int Any = -1;

    /// <summary>
    /// Destination CID used to refer to the hypervisor.
    /// </summary>
    public const int Hypervisor = 0;

    /// <summary>
    /// Loopback address for VSock sockets.
    /// </summary>
    public const int Local = 1;

    /// <summary>
    /// Host address for VSock sockets.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This context ID refers to any process running on the host, other than the hypervisor.
    /// </para>
    /// </remarks>
    public const int Host = 2;

    /// <summary>
    /// Creates a new VSock socket.
    /// </summary>
    /// <param name="type">One of the <see cref="SocketType"/> values.</param>
    /// <returns>The VSock socket.</returns>
    /// <exception cref="SocketException">
    /// An error occurred creating a socket with <paramref name="type"/>.
    /// </exception>
    /// <remarks>
    /// <para>
    ///   On the Linux version of .Net, the <see cref="Socket(AddressFamily, SocketType, ProtocolType)"/>
    ///   constructor verifies that the address family is one of the known supported members of the
    ///   <see cref="AddressFamily"/> enumeration, so this method uses PInvoke to create the socket.
    ///   It is not possible to create a VSock by instantiating the <see cref="Socket"/> class
    ///   directly.
    /// </para>
    /// <para>
    ///   As a result, some members of the <see cref="Socket"/> class, such as
    ///   <see cref="Socket.AddressFamily" qualifyHint="true"/> and
    ///   <see cref="Socket.SocketType" qualifyHint="true"/>, will not reflect accurate information.
    /// </para>
    /// </remarks>
    [SupportedOSPlatform("linux")]
    public static Socket Create(SocketType type)
    {
        var nativeSocket = socket((int)AddressFamily, (int)type, 0);
        if (nativeSocket.IsInvalid)
        {
            throw new SocketException(Marshal.GetLastPInvokeError());
        }

        return new Socket(nativeSocket);
    }

    [LibraryImport("libc", SetLastError = true)]
    private static partial SafeSocketHandle socket(int domain, int type, int protocol);
}

#endif
