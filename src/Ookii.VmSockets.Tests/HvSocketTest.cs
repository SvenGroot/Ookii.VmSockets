using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.System.Hypervisor;

namespace Ookii.VmSockets.Tests;

[TestClass]
public class HvSocketTest
{
    [TestMethod]
    public void TestConstants()
    {
        // Including the generated definitions from CsWin32 is overkill for the library, but it's
        // useful for testing that the constants are correct.
        Assert.AreEqual(PInvoke.AF_HYPERV, (int)HvSocket.AddressFamily);
        Assert.AreEqual(PInvoke.HV_PROTOCOL_RAW, (uint)HvSocket.RawProtocol);
        Assert.AreEqual(Marshal.SizeOf<SOCKADDR_HV>(), HvSocket.SocketAddressSize);
        Assert.AreEqual(PInvoke.HVSOCKET_CONNECT_TIMEOUT, (uint)HvSocket.SocketOption.ConnectTimeout);
        Assert.AreEqual(PInvoke.HVSOCKET_CONNECTED_SUSPEND, (uint)HvSocket.SocketOption.ConnectedSuspend);
        Assert.AreEqual(PInvoke.HVSOCKET_HIGH_VTL, (uint)HvSocket.SocketOption.HighVtl);
        Assert.AreEqual(PInvoke.HV_GUID_BROADCAST, HvSocket.Broadcast);
        Assert.AreEqual(PInvoke.HV_GUID_CHILDREN, HvSocket.Children);
        Assert.AreEqual(PInvoke.HV_GUID_LOOPBACK, HvSocket.Loopback);
        Assert.AreEqual(PInvoke.HV_GUID_PARENT, HvSocket.Parent);
        Assert.AreEqual(PInvoke.HV_GUID_SILOHOST, HvSocket.SiloHost);
        Assert.AreEqual(PInvoke.HV_GUID_VSOCK_TEMPLATE, HvSocket.VSockTemplate);
        Assert.AreEqual(PInvoke.HV_GUID_ZERO, HvSocket.Wildcard);
        Assert.AreEqual(HvSocket.VSockTemplate, HvSocket.CreateVSockServiceId(0));
        Assert.AreEqual(new Guid("000004D2-facb-11e6-bd58-64006a7986d3"), HvSocket.CreateVSockServiceId(1234));
    }

    [TestMethod]
    public void TestCreate()
    {
        if (!PlatformHelper.IsWindows10OrLater())
        {
            return;
        }

        using var socket = HvSocket.Create(SocketType.Stream);
        Assert.AreEqual(HvSocket.AddressFamily, socket.AddressFamily);
        Assert.AreEqual(HvSocket.RawProtocol, socket.ProtocolType);
        Assert.AreEqual(SocketType.Stream, socket.SocketType);
    }

    [TestMethod]
    public void TestConnectTimeout()
    {
        if (!PlatformHelper.IsWindows10_1607OrLater())
        {
            return;
        }

        using var socket = HvSocket.Create(SocketType.Stream);
        HvSocket.SetConnectTimeout(socket, 1234);
        Assert.AreEqual(1234, HvSocket.GetConnectTimeout(socket));
    }

    [TestMethod]
    public void TestConnectedSuspend()
    {
        if (!PlatformHelper.IsWindows10_1709OrLater())
        {
            return;
        }

        using var socket = HvSocket.Create(SocketType.Stream);
        HvSocket.SetConnectedSuspend(socket, true);
        Assert.IsTrue(HvSocket.GetConnectedSuspend(socket));
    }

    [TestMethod]
    public void TestContainerPassthrough()
    {
        if (!PlatformHelper.IsWindows10_1709OrLater(false) || PlatformHelper.IsWindows11_22h2OrLater(false))
        {
            Assert.Inconclusive("This test requires at least Windows 10 1709 and older than Windows 11 22h2.");
            return;
        }

        using var socket = HvSocket.Create(SocketType.Stream);
        HvSocket.SetContainerPassthru(socket, true);
        Assert.IsTrue(HvSocket.GetContainerPassthru(socket));
    }

    [TestMethod]
    public void TestHighVtl()
    {
        if (!PlatformHelper.IsWindows11_22h2OrLater())
        {
            return;
        }

        using var socket = HvSocket.Create(SocketType.Stream);
        HvSocket.SetHighVtl(socket, true);
        Assert.IsTrue(HvSocket.GetHighVtl(socket));
    }
}
