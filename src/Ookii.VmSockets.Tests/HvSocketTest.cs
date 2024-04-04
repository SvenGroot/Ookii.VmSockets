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
        Assert.AreEqual(Marshal.SizeOf<SOCKADDR_HV>(), HvSocket.SocketAddressSize);
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
}
