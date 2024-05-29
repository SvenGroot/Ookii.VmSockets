using Windows.Win32;

namespace Ookii.VmSockets.Tests;

[TestClass]
public class VmIdTest
{
    [TestMethod]
    public void TestConstants()
    {
        Assert.AreEqual(PInvoke.HV_GUID_BROADCAST, VmId.Broadcast);
        Assert.AreEqual(PInvoke.HV_GUID_CHILDREN, VmId.Children);
        Assert.AreEqual(PInvoke.HV_GUID_LOOPBACK, VmId.Loopback);
        Assert.AreEqual(PInvoke.HV_GUID_PARENT, VmId.Parent);
        Assert.AreEqual(PInvoke.HV_GUID_SILOHOST, VmId.SiloHost);
        Assert.AreEqual(PInvoke.HV_GUID_ZERO, VmId.Wildcard);
    }
}
