#if NET8_0_OR_GREATER

using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Ookii.VmSockets.Tests;

[TestClass]
public class VSockTest
{
    [TestMethod]
    public void TestConstants()
    {
        Assert.AreEqual(40, (int)VSock.AddressFamily);
        Assert.AreEqual(Marshal.SizeOf<PlatformHelper.sockaddr_vm>(), VSock.SocketAddressSize);
        Assert.AreEqual(-1, VSock.Any);
        Assert.AreEqual(0, VSock.Hypervisor);
        Assert.AreEqual(1, VSock.Local);
        Assert.AreEqual(2, VSock.Host);
    }

    [TestMethod]
    public void TestCreate()
    {
        if (!OperatingSystem.IsLinux())
        {
            Assert.Inconclusive("VSock sockets are only supported on Linux.");
            return;
        }

        using var socket = VSock.Create(SocketType.Stream);
        Assert.AreEqual(AddressFamily.Unknown, socket.AddressFamily);
    }

    [TestMethod]
    public void TestOptions()
    {
        if (!OperatingSystem.IsLinux())
        {
            Assert.Inconclusive("VSock sockets are only supported on Linux.");
            return;
        }

        using var socket = VSock.Create(SocketType.Stream);
        VSock.SetBufferSize(socket, 65536);
        Assert.AreEqual(65536, VSock.GetBufferSize(socket));

        VSock.SetBufferMinSize(socket, 1024);
        Assert.AreEqual(1024, VSock.GetBufferMinSize(socket));

        VSock.SetBufferMaxSize(socket, 1024 * 1024);
        Assert.AreEqual(1024 * 1024, VSock.GetBufferMaxSize(socket));

        VSock.SetConnectTimeout(socket, TimeSpan.FromSeconds(4.5));
        Assert.AreEqual(TimeSpan.FromSeconds(4.5), VSock.GetConnectTimeout(socket));

        // Additional options are not tested because they require a connected socket. This could
        // be done with a loopback connection, but that requires a specific kernel config which is
        // not guaranteed to be available (currently, it's not in WSL).
    }

    [TestMethod]
    public void TestGetLocalCid()
    {
        if (!OperatingSystem.IsLinux())
        {
            Assert.Inconclusive("VSock sockets are only supported on Linux.");
            return;
        }

        try
        {
            var cid = VSock.GetLocalCid();
            Assert.AreNotEqual(0, cid);
        }
        catch (UnauthorizedAccessException)
        {
            Assert.Inconclusive("This test requires root privileges.");
        }
    }
}

#endif
