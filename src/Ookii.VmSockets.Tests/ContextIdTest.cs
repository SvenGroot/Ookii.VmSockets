#if NET8_0_OR_GREATER

namespace Ookii.VmSockets.Tests;

[TestClass]
public class ContextIdTest
{
    [TestMethod]
    public void TestConstants()
    {
        Assert.AreEqual(-1, ContextId.Any);
        Assert.AreEqual(0, ContextId.Hypervisor);
        Assert.AreEqual(1, ContextId.Local);
        Assert.AreEqual(2, ContextId.Host);
    }
}

#endif
