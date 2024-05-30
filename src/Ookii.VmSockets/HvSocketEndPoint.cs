using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;

namespace Ookii.VmSockets;

/// <summary>
/// Represents a Hyper-V socket (hvsocket) endpoint as a VM ID and a service ID.
/// </summary>
/// <remarks>
/// <note type="important">
///   The functionality defined in this class is only available on Windows 10 and later versions.
/// </note>
/// <para>
///   Hyper-V sockets can be used to communicate between a host and a guest virtual machine or a
///   silo. It also provides interoperability with VSock sockets for virtual machines running Linux.
/// </para>
/// </remarks>
/// <threadsafety instance="false" static="true" />
/// <seealso href="https://learn.microsoft.com/virtualization/hyper-v-on-windows/user-guide/make-integration-service">Hyper-V socket integration services</seealso>
#if NET6_0_OR_GREATER
[SupportedOSPlatform("windows10.0")]
#endif
public class HvSocketEndPoint : EndPoint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HvSocketEndPoint"/> class.
    /// </summary>
    /// <param name="vmId">
    /// The ID of the virtual machine, or one of the well-known IDs defined in the
    /// <see cref="VmId"/> class.
    /// </param>
    /// <param name="serviceId">The service ID.</param>
    /// <remarks>
    /// <para>
    ///   To connect to a Linux guest that is using VSock sockets, use the
    ///   <see cref="HvSocketEndPoint(Guid, int)"/> constructor.
    /// </para>
    /// </remarks>
    public HvSocketEndPoint(Guid vmId, Guid serviceId)
    {
        VmId = vmId;
        ServiceId = serviceId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HvSocketEndPoint"/> class using a VSock port
    /// number.
    /// </summary>
    /// <param name="vmId">
    /// The ID of the virtual machine, or one of the well-known IDs defined in the
    /// <see cref="VmId"/> class.
    /// </param>
    /// <param name="port">The VSock port number. Valid values are between 0 and 0x7fffffff.</param>
    /// <remarks>
    /// <para>
    ///   This constructor can be used to create an endpoint that connects to a Linux guest that is
    ///   using VSock sockets.
    /// </para>
    /// </remarks>
    /// <seealso cref="HvSocket.CreateVSockServiceId(int)"/>
    public HvSocketEndPoint(Guid vmId, int port)
    {
        VmId = vmId;
        ServiceId = HvSocket.CreateVSockServiceId(port);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <value>
    /// The value of <c>AF_HYPERV</c>.
    /// </value>
    public override AddressFamily AddressFamily => HvSocket.AddressFamily;

    /// <summary>
    /// Gets or sets the virtual machine ID.
    /// </summary>
    /// <value>
    /// The ID of the virtual machine, or one of the well-known IDs defined in the
    /// <see cref="VmId"/> class.
    /// </value>
    public Guid VmId { get; set; }

    /// <summary>
    /// Gets or sets the service ID.
    /// </summary>
    /// <value>
    /// The service ID.
    /// </value>
    /// <remarks>
    /// <para>
    ///   To create a service ID that represents a VSock port number, use the
    ///   <see cref="HvSocket.CreateVSockServiceId(int)" qualifyHint="true"/> method, or the
    ///   <see cref="HvSocketEndPoint(Guid, int)"/> constructor.
    /// </para>
    /// </remarks>
    public Guid ServiceId { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="socketAddress"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    /// <remarks>
    /// <para>
    ///   The <paramref name="socketAddress"/> parameter must use the format of the
    ///   <c>SOCKADDR_HV</c> structure.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socketAddress"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The AddressFamily of <paramref name="socketAddress"/> is not <c>AF_HYPERV</c>, or the size
    /// of <paramref name="socketAddress"/> is less than
    /// <see cref="HvSocket.SocketAddressSize" qualifyHint="true"/>.
    /// </exception>
    public override EndPoint Create(SocketAddress socketAddress)
    {
        if (socketAddress == null)
        {
            throw new ArgumentNullException(nameof(socketAddress));
        }

        if (socketAddress.Family != AddressFamily)
        {
            throw new ArgumentException(Properties.Resources.InvalidAddressFamily, nameof(socketAddress));
        }

        if (socketAddress.Size < HvSocket.SocketAddressSize)
        {
            throw new ArgumentException(Properties.Resources.InvalidAddressSize, nameof(socketAddress));
        }

        var vmId = socketAddress.ReadGuid(4);
        var serviceId = socketAddress.ReadGuid(20);
        return new HvSocketEndPoint(vmId, serviceId);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    /// <remarks>
    /// <para>
    /// The returned <see cref="SocketAddress"/> will use the format of the <c>SOCKADDR_HV</c> structure.
    /// </para>
    /// </remarks>
    public override SocketAddress Serialize()
    {
        var address = new SocketAddress(AddressFamily, HvSocket.SocketAddressSize);
        address.WriteGuid(4, VmId);
        address.WriteGuid(20, ServiceId);
        return address;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    ///   There is no standard notation for Hyper-V socket addresses. This method returns a string
    ///   using the format <c>hvsocket[&lt;VmId&gt;:&lt;ServiceId&gt;]</c>.
    /// </para>
    /// </remarks>
    public override string ToString() => $"hvsocket[{VmId}:{ServiceId}]";

    /// <inheritdoc/>
    public override bool Equals(
#if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [NotNullWhen(true)]
#endif
    object? obj)
        => obj is HvSocketEndPoint endpoint && VmId == endpoint.VmId && ServiceId == endpoint.ServiceId;

    /// <inheritdoc/>
    public override int GetHashCode()
    {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        return HashCode.Combine(VmId, ServiceId);
#else
        return VmId.GetHashCode() ^ ServiceId.GetHashCode();
#endif
    }
}
