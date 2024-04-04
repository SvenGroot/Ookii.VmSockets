using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ookii.VmSockets.Tests;

[TestClass]
public class HvSocketEndPointTest
{
    [TestMethod]
    public void TestConstructor()
    {
        var vmId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var endpoint = new HvSocketEndPoint(vmId, serviceId);
        Assert.AreEqual(vmId, endpoint.VmId);
        Assert.AreEqual(serviceId, endpoint.ServiceId);

        endpoint = new HvSocketEndPoint(vmId, 1234);
        Assert.AreEqual(vmId, endpoint.VmId);
        Assert.AreEqual(new Guid("000004D2-facb-11e6-bd58-64006a7986d3"), endpoint.ServiceId);
    }
}
