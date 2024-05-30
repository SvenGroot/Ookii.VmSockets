namespace Ookii.VmSockets;

/// <summary>
/// Provides well-known VM IDs for Hyper-V sockets.
/// </summary>
/// <remarks>
/// <para>
///   For the equivalent for Linux VSock sockets, see the <see cref="ContextId"/> class.
/// </para>
/// </remarks>
/// <seealso cref="HvSocketEndPoint.VmId" qualifyHint="true"/>
/// <threadsafety instance="false" static="true" />
public static class VmId
{
    /// <summary>
    /// Broadcast address for Hyper-V sockets.
    /// </summary>
    public static readonly Guid Broadcast = new(0xFFFFFFFF, 0xFFFF, 0xFFFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);

    /// <summary>
    /// Wildcard address that refers to all the child partitions of the current host.
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
    ///   Using this <see cref="Guid"/> connects to the same partition as the connector.
    /// </para>
    /// </remarks>
    public static readonly Guid Loopback = new(0xE0E16197, 0xDD56, 0x4A10, 0x91, 0x95, 0x5E, 0xE7, 0xA1, 0x55, 0xA8, 0x38);

    /// <summary>
    /// Parent address for Hyper-V sockets.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Using this <see cref="Guid"/> connects or listens to the parent partition of the connector. The parent
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
    ///   Listeners should bind to this <see cref="Guid"/> to accept connection from all partitions.
    /// </para>
    /// </remarks>
    public static readonly Guid Wildcard = Guid.Empty;
}
