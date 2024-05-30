#if NET8_0_OR_GREATER

namespace Ookii.VmSockets;

/// <summary>
/// Provides well-known CIDs for Linux VSock sockets.
/// </summary>
/// <remarks>
/// <para>
///   For the equivalent for Hyper-V sockets, see the <see cref="VmId"/> class.
/// </para>
/// </remarks>
/// <threadsafety instance="false" static="true" />
/// <seealso cref="VSockEndPoint.ContextId" qualifyHint="true" />
public static class ContextId
{
    /// <summary>
    /// Wildcard CID for VSock sockets.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This value can be used as the CID in a <see cref="VSockEndPoint"/> to bind to any CID.
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
}

#endif
