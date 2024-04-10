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
}

#endif
