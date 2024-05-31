using System.Net.Sockets;
using System.Runtime.Versioning;

namespace Ookii.VmSockets;

/// <summary>
/// Provides constants and helper methods for working with Hyper-V sockets.
/// </summary>
/// <remarks>
/// <note type="important">
///   The functionality defined in this class is only available on Windows 10 and later versions.
/// </note>
/// </remarks>
/// <threadsafety instance="false" static="true" />
/// <seealso href="https://learn.microsoft.com/virtualization/hyper-v-on-windows/user-guide/make-integration-service">Hyper-V socket integration services</seealso>
public static class HvSocket
{
    /// <summary>
    /// Defines socket options available for Hyper-V sockets.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   All these options use the value of <see cref="RawProtocol"/> as the level. Use the
    ///   helper methods in the <see cref="HvSocket"/> class to get and set these options.
    /// </para>
    /// </remarks>
    public enum SocketOption
    {
        /// <summary>
        /// Sets the connection timeout for the socket in milliseconds. Type: <see cref="uint"/>.
        /// </summary>
        ConnectTimeout = 1,
        /// <summary>
        /// Indicates the connection will pass through to a container. A non-zero value indicates
        /// the option is enabled. Type: <see cref="uint"/>.
        /// </summary>
        ContainerPassthru = 2,
        /// <summary>
        /// Indicates that the socket will not disconnect when the VM is paused. A non-zero value
        /// indicates the option is enabled. Type: <see cref="uint"/>.
        /// </summary>
        ConnectedSuspend = 4,
        /// <summary>
        /// Indicates that the socket will connect to the guest's management VTL. A non-zero value
        /// indicates the option is enabled. Type: <see cref="uint"/>.
        /// </summary>
        HighVtl = 8
    }

    /// <summary>
    /// The address family for Hyper-V sockets, also known as <c>AF_HYPERV</c>.
    /// </summary>
    public const AddressFamily AddressFamily = (AddressFamily)34;

    /// <summary>
    /// The protocol type that must be used when creating Hyper-V sockets.
    /// </summary>
    public const ProtocolType RawProtocol = (ProtocolType)1;

    /// <summary>
    /// The size, in bytes, of the <c>SOCKADDR_HV</c> structure, used to describe Hyper-V socket
    /// addresses.
    /// </summary>
    public const int SocketAddressSize = 36;

    /// <summary>
    /// Template to facilitate a mapping between the service ID of a Hyper-V socket and the port
    /// number of a Linux VSock socket.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   To create a service ID for a VSock port number, use the <see cref="CreateVSockServiceId"/>
    ///   method, or use the <see cref="HvSocketEndPoint(Guid, int)"/> constructor.
    /// </para>
    /// </remarks>
    public static readonly Guid VSockTemplate = new(0x00000000, 0xFACB, 0x11E6, 0xBD, 0x58, 0x64, 0x00, 0x6A, 0x79, 0x86, 0xD3);

    /// <summary>
    /// Maps the port number of a Linux VSock socket to a service ID for a Hyper-V socket.
    /// </summary>
    /// <param name="port">The VSock port number. Valid values are between 0 and 0x7fffffff.</param>
    /// <returns>The mapped service ID.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="port"/> is less than 0.
    /// </exception>
    public static Guid CreateVSockServiceId(int port)
    {
        if (port < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(port));
        }

        return new((uint)port, 0xFACB, 0x11E6, 0xBD, 0x58, 0x64, 0x00, 0x6A, 0x79, 0x86, 0xD3);
    }

    /// <summary>
    /// Creates a new Hyper-V socket.
    /// </summary>
    /// <param name="type">One of the <see cref="SocketType"/> values.</param>
    /// <returns>The Hyper-V socket.</returns>
    /// <remarks>
    /// <para>
    ///   This method is equivalent to invoking the <see cref="Socket(AddressFamily, SocketType, ProtocolType)"/>
    ///   constructor using the values of <see cref="AddressFamily"/>, <paramref name="type"/>, and
    ///   <see cref="RawProtocol"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="SocketException">The socket could not be created.</exception>
#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows10.0")]
#endif
    public static Socket Create(SocketType type) => new(AddressFamily, type, RawProtocol);

    /// <summary>
    /// Sets the connection timeout for the socket.
    /// </summary>
    /// <param name="socket">A Hyper-V socket.</param>
    /// <param name="timeout">The timeout.</param>
    /// <remarks>
    /// <para>
    ///   This socket option was introduced in Windows 10, version 1607 (build 14393).
    /// </para>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a Hyper-V socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// The <see cref="Socket"/> object has been closed.
    /// </exception>
    /// <exception cref="SocketException">
    /// An error occurred when attempting to access the socket.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The value of <paramref name="timeout"/> in milliseconds is less than 0 or greater than <see cref="int.MaxValue"/>.
    /// </exception>
#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows10.0.14393")]
#endif
    public static void SetConnectTimeout(Socket socket, TimeSpan timeout)
    {
        if (socket == null)
        {
            throw new ArgumentNullException(nameof(socket));
        }

        if (timeout.TotalMilliseconds is < 0 or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout));
        }

        socket.SetSocketOption((SocketOptionLevel)RawProtocol, (SocketOptionName)SocketOption.ConnectTimeout, (int)timeout.TotalMilliseconds);
    }

    /// <summary>
    /// Gets the connection timeout for the socket.
    /// </summary>
    /// <param name="socket">A Hyper-V socket.</param>
    /// <returns>The timeout.</returns>
    /// <remarks>
    /// <para>
    ///   This socket option was introduced in Windows 10, version 1607 (build 14393).
    /// </para>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a Hyper-V socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// The <see cref="Socket"/> object has been closed.
    /// </exception>
    /// <exception cref="SocketException">
    /// An error occurred when attempting to access the socket.
    /// </exception>
#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows10.0.14393")]
#endif
    public static TimeSpan GetConnectTimeout(Socket socket)
    {
        if (socket == null)
        {
            throw new ArgumentNullException(nameof(socket));
        }

        var timeout = (int)socket.GetSocketOption((SocketOptionLevel)RawProtocol, (SocketOptionName)SocketOption.ConnectTimeout)!;
        return TimeSpan.FromMilliseconds(timeout);
    }

    /// <summary>
    /// Sets the container pass-through socket option.
    /// </summary>
    /// <param name="socket">A Hyper-V socket.</param>
    /// <param name="value">
    /// <see langword="true"/> to enable pass-through; otherwise, <see langword="false"/>.
    /// </param>
    /// <remarks>
    /// <para>
    ///   This socket option was introduced in Windows 10, version 1607 (build 14393), and was
    ///   removed in Windows 11, version 22h2 (build 22621).
    /// </para>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a Hyper-V socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// The <see cref="Socket"/> object has been closed.
    /// </exception>
    /// <exception cref="SocketException">
    /// An error occurred when attempting to access the socket.
    /// </exception>
#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows10.0.14393")]
    [UnsupportedOSPlatform("windows10.0.22621")]
#endif
    public static void SetContainerPassthru(Socket socket, bool value)
    {
        if (socket == null)
        {
            throw new ArgumentNullException(nameof(socket));
        }

        socket.SetSocketOption((SocketOptionLevel)RawProtocol, (SocketOptionName)SocketOption.ContainerPassthru, value);
    }

    /// <summary>
    /// Gets the container pass-through socket option.
    /// </summary>
    /// <param name="socket">A Hyper-V socket.</param>
    /// <returns>
    /// <see langword="true"/> to indicate container pass-through is enabled; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    ///   This socket option was introduced in Windows 10, version 1607 (build 14393), and was
    ///   removed in Windows 11, version 22h2 (build 22621).
    /// </para>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a Hyper-V socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// The <see cref="Socket"/> object has been closed.
    /// </exception>
    /// <exception cref="SocketException">
    /// An error occurred when attempting to access the socket.
    /// </exception>
#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows10.0.14393")]
    [UnsupportedOSPlatform("windows10.0.22621")]
#endif
    public static bool GetContainerPassthru(Socket socket)
    {
        if (socket == null)
        {
            throw new ArgumentNullException(nameof(socket));
        }

        return (int)socket.GetSocketOption((SocketOptionLevel)RawProtocol, (SocketOptionName)SocketOption.ContainerPassthru)! != 0;
    }

    /// <summary>
    /// Sets the socket connected suspend option.
    /// </summary>
    /// <param name="socket">A Hyper-V socket.</param>
    /// <param name="value">
    /// <see langword="true"/> to indicate the socket will not disconnect when the VM is paused;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <remarks>
    /// <para>
    ///   This socket option was introduced in Windows 10, version 1709 (build 16299).
    /// </para>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a Hyper-V socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// The <see cref="Socket"/> object has been closed.
    /// </exception>
    /// <exception cref="SocketException">
    /// An error occurred when attempting to access the socket.
    /// </exception>
#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows10.0.16299")]
#endif
    public static void SetConnectedSuspend(Socket socket, bool value)
    {
        if (socket == null)
        {
            throw new ArgumentNullException(nameof(socket));
        }

        socket.SetSocketOption((SocketOptionLevel)RawProtocol, (SocketOptionName)SocketOption.ConnectedSuspend, value);
    }

    /// <summary>
    /// Gets the socket connected suspend option.
    /// </summary>
    /// <param name="socket">A Hyper-V socket.</param>
    /// <returns>
    /// <see langword="true"/> to indicate the socket will not disconnect when the VM is paused;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    ///   This socket option was introduced in Windows 10, version 1709 (build 16299).
    /// </para>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a Hyper-V socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// The <see cref="Socket"/> object has been closed.
    /// </exception>
    /// <exception cref="SocketException">
    /// An error occurred when attempting to access the socket.
    /// </exception>
#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows10.0.16299")]
#endif
    public static bool GetConnectedSuspend(Socket socket)
    {
        if (socket == null)
        {
            throw new ArgumentNullException(nameof(socket));
        }

        return (int)socket.GetSocketOption((SocketOptionLevel)RawProtocol, (SocketOptionName)SocketOption.ConnectedSuspend)! != 0;
    }

    /// <summary>
    /// Sets the socket high VTL option.
    /// </summary>
    /// <param name="socket">A Hyper-V socket.</param>
    /// <param name="value">
    /// <see langword="true"/> to indicate the socket will connect to the guest's management VTL;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <remarks>
    /// <para>
    ///   This socket option was introduced in Windows 11, version 22h2 (build 22621).
    /// </para>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a Hyper-V socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// The <see cref="Socket"/> object has been closed.
    /// </exception>
    /// <exception cref="SocketException">
    /// An error occurred when attempting to access the socket.
    /// </exception>
#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows10.0.22621")]
#endif
    public static void SetHighVtl(Socket socket, bool value)
    {
        if (socket == null)
        {
            throw new ArgumentNullException(nameof(socket));
        }

        socket.SetSocketOption((SocketOptionLevel)RawProtocol, (SocketOptionName)SocketOption.HighVtl, value);
    }

    /// <summary>
    /// Gets the socket high VTL option.
    /// </summary>
    /// <param name="socket">A Hyper-V socket.</param>
    /// <returns>
    /// <see langword="true"/> to indicate the socket will connect to the guest's management VTL;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    ///   This socket option was introduced in Windows 11, version 22h2 (build 22621).
    /// </para>
    /// <para>
    ///   The behavior of this function is undefined if the socket is not a Hyper-V socket.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socket"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// The <see cref="Socket"/> object has been closed.
    /// </exception>
    /// <exception cref="SocketException">
    /// An error occurred when attempting to access the socket.
    /// </exception>
#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows10.0.22621")]
#endif
    public static bool GetHighVtl(Socket socket)
    {
        if (socket == null)
        {
            throw new ArgumentNullException(nameof(socket));
        }

        return (int)socket.GetSocketOption((SocketOptionLevel)RawProtocol, (SocketOptionName)SocketOption.HighVtl)! != 0;
    }
}
