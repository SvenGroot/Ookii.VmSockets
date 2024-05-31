#if NET8_0_OR_GREATER

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;

namespace Ookii.VmSockets;

/// <summary>
/// Represents a Linux VSock socket endpoint as a context ID and a port.
/// </summary>
/// <remarks>
/// <note type="important">
///   The functionality defined in this class is only available on Linux kernels that support the
///   <c>AF_VSOCK</c> address family.
/// </note>
/// <note type="important">
///   Due to limitations in the Sockets API on the Linux version of .Net, this class is only
///   available for .Net 8.0 and later.
/// </note>
/// <para>
///   VSock sockets can be used between a host and a guest virtual machine. It also provides
///   interoperability with Hyper-V sockets for guest virtual machines running Linux on a Windows
///   host.
/// </para>
/// </remarks>
/// <threadsafety instance="false" static="true" />
[SupportedOSPlatform("linux")]
public class VSockEndPoint : EndPoint
{
    private const byte ToHostFlag = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="VSockEndPoint"/> class.
    /// </summary>
    /// <param name="contextId">
    /// Context ID identifying the destination. This can be one of the well known CID values
    /// defined in the <see cref="VmSockets.ContextId"/> class.
    /// </param>
    /// <param name="port">
    /// The port number, or <see cref="VSock.AnyPort" qualifyHint="true"/> to bind to any available
    /// port.
    /// </param>
    public VSockEndPoint(int contextId = VmSockets.ContextId.Any, int port = VSock.AnyPort)
    {
        ContextId = contextId;
        Port = port;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <value>
    /// The value of <c>AF_VSOCK</c>.
    /// </value>
    public override AddressFamily AddressFamily => VSock.AddressFamily;

    /// <summary>
    /// Gets or sets the context ID identifying the destination.
    /// </summary>
    /// <value>
    /// A context ID referring to a specific partition, or one of the well known CID values defined
    /// in the <see cref="VmSockets.ContextId"/> class.
    /// </value>
    public int ContextId { get; set; }

    /// <summary>
    /// Gets or sets the port number.
    /// </summary>
    /// <value>
    /// The port number, or <see cref="VSock.AnyPort" qualifyHint="true"/> to bind to any available
    /// port.
    /// </value>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets a value which indicates that VSock packets need to always be forwarded to
    /// the host.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if VSock packets need to always be forwarded to the host; otherwise,
    /// <see langword="false"/>. The default value is <see langword="false"/>.
    /// </value>
    public bool ToHost { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="socketAddress"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    /// <remarks>
    /// <para>
    ///   The <paramref name="socketAddress"/> parameter must use the format of the
    ///   <c>sockaddr_vm</c> structure.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="socketAddress"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The AddressFamily of <paramref name="socketAddress"/> is not <c>AF_HYPERV</c>, or the size
    /// of <paramref name="socketAddress"/> is less than <see cref="VSock.SocketAddressSize" qualifyHint="true"/>.
    /// </exception>
    public override EndPoint Create(SocketAddress socketAddress)
    {
        ArgumentNullException.ThrowIfNull(socketAddress);

        // The Family property doesn't give the right value if the family in the buffer is not a
        // known value, so we need to check the buffer directly.
        if (socketAddress.GetRealAddressFamily() != AddressFamily)
        {
            throw new ArgumentException(Properties.Resources.InvalidAddressFamily, nameof(socketAddress));
        }

        if (socketAddress.Size < VSock.SocketAddressSize)
        {
            throw new ArgumentException(Properties.Resources.InvalidAddressSize, nameof(socketAddress));
        }

        var contextId = BitConverter.ToInt32(socketAddress.Buffer.Span[8..12]);
        var port = BitConverter.ToInt32(socketAddress.Buffer.Span[4..8]);
        var toHost = (socketAddress.Buffer.Span[12] | ToHostFlag) != 0;

        return new VSockEndPoint(contextId, port) { ToHost = toHost };
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    /// <remarks>
    /// <para>
    ///   The returned <see cref="SocketAddress"/> will use the format of the <c>sockaddr_vm</c>
    ///   structure.
    /// </para>
    /// <note type="important">
    ///   Due to limitations in the Linux version of .Net, the <see cref="SocketAddress.Family" qualifyHint="true"/>
    ///   property will not return the correct value. The correct value can be obtained by reading
    ///   the first two bytes of the buffer.
    /// </note>
    /// </remarks>
    public override SocketAddress Serialize()
    {
        // This is the reason why we need to use .Net 8 or later.
        //
        // On Linux, the SocketAddress class validates the address family is one of the known
        // supported values, so it cannot be created using AF_VSOCK. This can be worked around
        // by overwriting the address family in the buffer, but this is not possible prior to
        // .Net 8, because the SocketAddress class does not expose the buffer and the indexer
        // doesn't provide write access to the first two bytes.
        var address = new SocketAddress(AddressFamily.Unspecified, VSock.SocketAddressSize);

        // Replace the address family.
        BitConverter.TryWriteBytes(address.Buffer.Span[0..2], (ushort)AddressFamily);

        // Set the svm_cid and svm_port fields.
        BitConverter.TryWriteBytes(address.Buffer.Span[4..8], Port);
        BitConverter.TryWriteBytes(address.Buffer.Span[8..12], ContextId);

        // Set the svm_flags field if needed.
        if (ToHost)
        {
            address.Buffer.Span[12] = ToHostFlag;
        }

        return address;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    ///   There is no standard notation for VSock socket addresses. This method returns a string
    ///   using the format <c>vsock[&lt;ContextId&gt;:&lt;Port&gt;]</c>.
    /// </para>
    /// </remarks>
    public override string ToString() => $"vsock[{ContextId}:{Port}]";

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is VSockEndPoint other && ContextId == other.ContextId && Port == other.Port && ToHost == other.ToHost;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(ContextId, Port, ToHost);
}

#endif
