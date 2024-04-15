#if NET8_0_OR_GREATER

using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;

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
    /// Defines socket options available for VSock sockets.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   All these options use the value of <see cref="AddressFamily"/> as the level. Use the
    ///   helper functions in this class to get and set these options.
    /// </para>
    /// </remarks>
    public enum SocketOption
    {
        /// <summary>
        /// The buffer size for a stream socket. The value will be clamped to the minimum and
        /// maximum buffer sizes. Type: UInt64.
        /// </summary>
        BufferSize = 0,
        /// <summary>
        /// The minimum buffer size for a stream socket. Type: UInt64.
        /// </summary>
        BufferMinSize = 1,
        /// <summary>
        /// The maximum buffer size for a stream socket. Type: UInt64.
        /// </summary>
        BufferMaxSize = 2,
        /// <summary>
        /// Retrieves the host-specific VM ID of the peer. Type: Int32.
        /// </summary>
        PeerHostVmId = 3,
        /// <summary>
        /// Determines if a socket is trusted. A non-zero value indicates that the socket is
        /// trusted. Type: Int32.
        /// </summary>
        Trusted = 5,
        /// <summary>
        /// The connection timeout for a stream socket. Type: Int32.
        /// </summary>
        ConnectTimeout = 6,
    }

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

    /// <summary>
    /// Sets the buffer size for a stream socket.
    /// </summary>
    /// <param name="socket">A VSock socket.</param>
    /// <param name="size">
    /// The buffer size in bytes. The value will be clamped between the minimum and maximum buffer size.
    /// </param>
    /// <remarks>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a VSock socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    [SupportedOSPlatform("linux")]
    public static void SetBufferSize(Socket socket, long size) => SetSocketOption(socket, SocketOption.BufferSize, size);

    /// <summary>
    /// Gets the buffer size for a stream socket.
    /// </summary>
    /// <param name="socket">A VSock socket.</param>
    /// <returns>The buffer size in bytes.</returns>
    /// <remarks>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a VSock socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    [SupportedOSPlatform("linux")]
    public static long GetBufferSize(Socket socket) => GetSocketOption<long>(socket, SocketOption.BufferSize);

    /// <summary>
    /// Sets the minimum buffer size for a stream socket.
    /// </summary>
    /// <param name="socket">A VSock socket.</param>
    /// <param name="size">The buffer size in bytes.</param>
    /// <remarks>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a VSock socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    [SupportedOSPlatform("linux")]
    public static void SetBufferMinSize(Socket socket, long size) => SetSocketOption(socket, SocketOption.BufferMinSize, size);

    /// <summary>
    /// Gets the minimum buffer size for a stream socket.
    /// </summary>
    /// <param name="socket">A VSock socket.</param>
    /// <returns>The buffer size in bytes.</returns>
    /// <remarks>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a VSock socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    [SupportedOSPlatform("linux")]
    public static long GetBufferMinSize(Socket socket) => GetSocketOption<long>(socket, SocketOption.BufferMinSize);

    /// <summary>
    /// Sets the maximum buffer size for a stream socket.
    /// </summary>
    /// <param name="socket">A VSock socket.</param>
    /// <param name="size">The buffer size in bytes.</param>
    /// <remarks>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a VSock socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    [SupportedOSPlatform("linux")]
    public static void SetBufferMaxSize(Socket socket, long size) => SetSocketOption(socket, SocketOption.BufferMaxSize, size);

    /// <summary>
    /// Gets the maximum buffer size for a stream socket.
    /// </summary>
    /// <param name="socket">A VSock socket.</param>
    /// <returns>The buffer size in bytes.</returns>
    /// <remarks>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a VSock socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    [SupportedOSPlatform("linux")]
    public static long GetBufferMaxSize(Socket socket) => GetSocketOption<long>(socket, SocketOption.BufferMaxSize);

    /// <summary>
    /// Gets the host-specific VM ID of the peer.
    /// </summary>
    /// <param name="socket">A VSock socket.</param>
    /// <returns>The peer VM ID.</returns>
    /// <remarks>
    /// <para>
    ///   This option is only available for hypervisor endpoints.
    /// </para>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a VSock socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    public static int GetPeerHostVmId(Socket socket) => GetSocketOption<int>(socket, SocketOption.PeerHostVmId);

    /// <summary>
    /// Gets the host-specific VM ID of the peer.
    /// </summary>
    /// <param name="socket">A VSock socket.</param>
    /// <returns>The peer VM ID.</returns>
    /// <remarks>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a VSock socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    public static bool GetTrusted(Socket socket) => GetSocketOption<int>(socket, SocketOption.Trusted) != 0;

    /// <summary>
    /// Sets the connect timeout for a stream socket.
    /// </summary>
    /// <param name="socket">A VSock socket.</param>
    /// <param name="timeout">The timeout.</param>
    /// <remarks>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a VSock socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    [SupportedOSPlatform("linux")]
    public static void SetConnectTimeout(Socket socket, TimeSpan timeout) => SetSocketOption(socket, SocketOption.ConnectTimeout, new Timeval(timeout));

    /// <summary>
    /// Gets the connect timeout for a stream socket.
    /// </summary>
    /// <param name="socket">A VSock socket.</param>
    /// <returns>The timeout.</returns>
    /// <remarks>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a VSock socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    [SupportedOSPlatform("linux")]
    public static TimeSpan GetConnectTimeout(Socket socket) => GetSocketOption<Timeval>(socket, SocketOption.ConnectTimeout).ToTimeSpan();

    /// <summary>
    /// Gets the CID of the local machine.
    /// </summary>
    /// <returns>The context ID.</returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Retrieving the local context ID requires root privileges.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// The /dev/vsock device was not found.
    /// </exception>
    /// <exception cref="Win32Exception">
    /// An error occurred retrieving the local CID.
    /// </exception>
    [SupportedOSPlatform("linux")]
    public static int GetLocalCid()
    {

        int value = 0;
        using var file = open(VSockDevice, 0);
        if (file.IsInvalid)
        {
            ThrowExceptionForLastError();
        }

        if (ioctl(file, IOCTL_VM_SOCKETS_GET_LOCAL_CID, ref value) < 0)
        {
            ThrowExceptionForLastError();
        }

        return value;
    }

    private static void SetSocketOption<T>(Socket socket, SocketOption option, T value)
        where T: struct
    {
        ArgumentNullException.ThrowIfNull(socket);

        var buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1));
        socket.SetRawSocketOption((int)AddressFamily, (int)option, buffer);
    }

    private static T GetSocketOption<T>(Socket socket, SocketOption option)
        where T: struct
    {
        ArgumentNullException.ThrowIfNull(socket);

        T result = default;
        var buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref result, 1));
        socket.GetRawSocketOption((int)AddressFamily, (int)option, buffer);
        return result;
    }

    [DoesNotReturn]
    private static void ThrowExceptionForLastError()
    {
        var error = Marshal.GetLastPInvokeError();
        throw error switch
        {
            EPERM => new UnauthorizedAccessException(),
            ENOENT => new FileNotFoundException(null, VSockDevice),
            EACCES => new UnauthorizedAccessException(),
            _ => new Win32Exception(error),
        };
    }

    #region PInvoke

    private const int IOCTL_VM_SOCKETS_GET_LOCAL_CID = 0x7b9;

    private const int EPERM = 1;
    private const int ENOENT = 2;
    private const int EACCES = 13;

    private const string VSockDevice = "/dev/vsock";

    [LibraryImport("libc", SetLastError = true)]
    private static partial SafeSocketHandle socket(int domain, int type, int protocol);

    [LibraryImport("libc", SetLastError = true)]
    private static partial SafeFileHandle open([MarshalAs(UnmanagedType.LPStr)] string path, int flags);

    [LibraryImport("libc", SetLastError = true)]
    private static partial int ioctl(SafeFileHandle socket, int request, ref int value);

    [StructLayout(LayoutKind.Sequential)]
    private struct Timeval
    {
        public Timeval(TimeSpan value)
        {
            tv_sec = (long)value.TotalSeconds;
            tv_usec = value.Milliseconds * 1000 + value.Microseconds;
        }

        public readonly TimeSpan ToTimeSpan() => TimeSpan.FromSeconds(tv_sec) + TimeSpan.FromMicroseconds(tv_usec);

        public long tv_sec;
        public long tv_usec;
    }

    #endregion
}

#endif
