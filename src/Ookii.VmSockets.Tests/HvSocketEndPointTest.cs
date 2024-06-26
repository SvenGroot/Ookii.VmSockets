﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Networking.WinSock;
using Windows.Win32.System.Hypervisor;

namespace Ookii.VmSockets.Tests;

[TestClass]
public class HvSocketEndPointTest
{
    [TestMethod]
    public void TestConstructor()
    {
        if (!PlatformHelper.IsWindows10OrLater())
        {
            return;
        }

        var vmId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var endpoint = new HvSocketEndPoint(vmId, serviceId);
        Assert.AreEqual(vmId, endpoint.VmId);
        Assert.AreEqual(serviceId, endpoint.ServiceId);

        endpoint = new HvSocketEndPoint(vmId, 1234);
        Assert.AreEqual(vmId, endpoint.VmId);
        Assert.AreEqual(new Guid("000004D2-facb-11e6-bd58-64006a7986d3"), endpoint.ServiceId);
    }

    [TestMethod]
    public void TestSerialize()
    {
        if (!PlatformHelper.IsWindows10OrLater())
        {
            return;
        }

        var vmId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var endpoint = new HvSocketEndPoint(vmId, serviceId);
        var socketAddress = endpoint.Serialize();
        var buffer = socketAddress.GetBuffer();
        var expected = new SOCKADDR_HV()
        {
            Family = (ADDRESS_FAMILY)PInvoke.AF_HYPERV,
            Reserved = 0,
            VmId = vmId,
            ServiceId = serviceId,
        };

        ReadOnlySpan<SOCKADDR_HV> span;
        unsafe
        {
            span = new ReadOnlySpan<SOCKADDR_HV>(&expected, 1);
        }

        var expectedBuffer = MemoryMarshal.AsBytes(span);
        CollectionAssert.AreEqual(expectedBuffer.ToArray(), buffer.ToArray());
    }

    [TestMethod]
    public void TestCreate()
    {
        if (!PlatformHelper.IsWindows10OrLater())
        {
            return;
        }

        var vmId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var endpoint = new HvSocketEndPoint(vmId, serviceId);
        var socketAddress = endpoint.Serialize();
        var endpoint2 = new HvSocketEndPoint(Guid.Empty, Guid.Empty);
        endpoint2 = (HvSocketEndPoint)endpoint2.Create(socketAddress);
        Assert.AreEqual(endpoint.VmId, endpoint2.VmId);
        Assert.AreEqual(endpoint.ServiceId, endpoint2.ServiceId);
    }

    [TestMethod]
    public void TestEqualsAndGetHashCode()
    {
        if (!PlatformHelper.IsWindows10OrLater())
        {
            return;
        }

        var vmId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var endpoint = new HvSocketEndPoint(vmId, serviceId);
        var endpoint2 = new HvSocketEndPoint(vmId, serviceId);
        Assert.AreEqual(endpoint, endpoint2);
        Assert.AreEqual(endpoint.GetHashCode(), endpoint2.GetHashCode());
        endpoint2.VmId = Guid.NewGuid();
        Assert.AreNotEqual(endpoint, endpoint2);
        Assert.AreNotEqual(endpoint.GetHashCode(), endpoint2.GetHashCode());
        endpoint2.VmId = vmId;
        endpoint2.ServiceId = Guid.NewGuid();
        Assert.AreNotEqual(endpoint, endpoint2);
        Assert.AreNotEqual(endpoint.GetHashCode(), endpoint2.GetHashCode());

        Assert.IsFalse(endpoint.Equals(null));
        Assert.IsFalse(endpoint.Equals("foo"));
    }

    [TestMethod]
    public void TestToString()
    {
        if (!PlatformHelper.IsWindows10OrLater())
        {
            return;
        }

        var vmId = new Guid("{502852EA-3B70-42EF-AF05-D41EA3A32221}");
        var serviceId = new Guid("{17743CBE-8D4B-4463-9C38-57603A85B29B}");

        var endpoint = new HvSocketEndPoint(vmId, serviceId);
        Assert.AreEqual("hvsocket[502852ea-3b70-42ef-af05-d41ea3a32221:17743cbe-8d4b-4463-9c38-57603a85b29b]", endpoint.ToString());
    }
}
