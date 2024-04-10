using System.Net.Sockets;
using System.Runtime.Versioning;

namespace Ookii.VmSockets;

/// <summary>
/// Provides constants and helper methods for working with Hyper-V sockets.
/// </summary>
public static class HvSocket
{
    /// <summary>
    /// The address family for Hyper-V sockets, also known as <c>AF_HYPERV</c>.
    /// </summary>
    public const AddressFamily AddressFamily = (AddressFamily)34;

    /// <summary>
    /// The protocol type that must be used when creating Hyper-V sockets.
    /// </summary>
    public const ProtocolType RawProtocol = (ProtocolType)1;

    /// <summary>
    /// The size of the <c>SOCKADDR_HV</c> structure, used to describe Hyper-V socket addresses.
    /// </summary>
    public const int SocketAddressSize = 36;

    /// <summary>
    /// Broadcast address for Hyper-V sockets.
    /// </summary>
    public static readonly Guid Broadcast = new(0xFFFFFFFF, 0xFFFF, 0xFFFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);

    /// <summary>
    /// Wildcard address for children.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Listeners should bind to this VM ID to accept connections from its children.
    /// </para>
    /// </remarks>
    public static readonly Guid Children = new(0x90DB8B89, 0x0D35, 0x4F79, 0x8C, 0xE9, 0x49, 0xEA, 0x0A, 0xC8, 0xB7, 0xCD);

    /// <summary>
    /// Loopback address for Hyper-V sockets.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Using this VmId connects to the same partition as the connector.
    /// </para>
    /// </remarks>
    public static readonly Guid Loopback = new(0xE0E16197, 0xDD56, 0x4A10, 0x91, 0x95, 0x5E, 0xE7, 0xA1, 0x55, 0xA8, 0x38);

    /// <summary>
    /// Parent address for Hyper-V sockets.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Using this VmId connects or listens to the parent partition of the connector. The parent
    ///   of a virtual machine is its host. The parent of a hosted silo is the VM's host.
    /// </para>
    /// </remarks>
    public static readonly Guid Parent = new(0xA42E7CDA, 0xD03F, 0x480C, 0x9C, 0xC2, 0xA4, 0xDE, 0x20, 0xAB, 0xB8, 0x78);

    /// <summary>
    /// Address of a silo's host partition.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The silo host of a hosted silo is the utility VM. The silo host of a silo on a physical
    ///   host is the physical host.
    /// </para>
    /// </remarks>
    public static readonly Guid SiloHost = new(0x36BD0C5C, 0x7276, 0x4223, 0x88, 0xBA, 0x7D, 0x03, 0xB6, 0x54, 0xC5, 0x68);

    /// <summary>
    /// Wildcard address for Hyper-V sockets.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Listeners should bind to this VmId to accept connection from all partitions.
    /// </para>
    /// </remarks>
    public static readonly Guid Wildcard = Guid.Empty;

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
#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public static Socket Create(SocketType type) => new(AddressFamily, type, RawProtocol);
}
