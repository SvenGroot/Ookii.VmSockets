﻿#if NET8_0_OR_GREATER

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Ookii.VmSockets.Tests;

[TestClass]
public class VSockEndPointTests
{
    [TestMethod]
    public void TestConstructor()
    {
        if (!OperatingSystem.IsLinux())
        {
            Assert.Inconclusive("VSock sockets are only supported on Linux.");
            return;
        }

        var endpoint = new VSockEndPoint(ContextId.Host, 12345);
        Assert.AreEqual(ContextId.Host, endpoint.ContextId);
        Assert.AreEqual(12345, endpoint.Port);
        Assert.IsFalse(endpoint.ToHost);
    }

    [TestMethod]
    public void TestSerialize()
    {
        if (!OperatingSystem.IsLinux())
        {
            Assert.Inconclusive("VSock sockets are only supported on Linux.");
            return;
        }

        var endpoint = new VSockEndPoint(ContextId.Host, 12345) { ToHost = true };
        var socketAddress = endpoint.Serialize();
        var buffer = socketAddress.Buffer;
        var expected = new PlatformHelper.sockaddr_vm()
        {
            svm_family = 40,
            svm_cid = ContextId.Host,
            svm_port = 12345,
            svm_flags = 1,
        };

        // I don't know why this creates a span that is bigger than sizeof(sockaddr_vm), but it
        // does, so take only the relevant part.
        var expectedBuffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref expected, 1))[0..16];
        CollectionAssert.AreEqual(expectedBuffer.ToArray(), buffer.ToArray());
    }

    [TestMethod]
    public void TestCreate()
    {
        if (!OperatingSystem.IsLinux())
        {
            Assert.Inconclusive("VSock sockets are only supported on Linux.");
            return;
        }

        var endpoint = new VSockEndPoint(ContextId.Local, 12345) { ToHost = true };
        var socketAddress = endpoint.Serialize();
        var endpoint2 = new VSockEndPoint();
        endpoint2 = (VSockEndPoint)endpoint2.Create(socketAddress);
        Assert.AreEqual(endpoint.ContextId, endpoint2.ContextId);
        Assert.AreEqual(endpoint.Port, endpoint2.Port);
        Assert.AreEqual(endpoint.ToHost, endpoint2.ToHost);
    }

    [TestMethod]
    public void TestEqualsAndGetHashCode()
    {
        if (!OperatingSystem.IsLinux())
        {
            Assert.Inconclusive("VSock sockets are only supported on Linux.");
            return;
        }

        var endpoint = new VSockEndPoint(ContextId.Host, 12345);
        var endpoint2 = new VSockEndPoint(ContextId.Host, 12345);
        Assert.AreEqual(endpoint, endpoint2);
        Assert.AreEqual(endpoint.GetHashCode(), endpoint2.GetHashCode());
        endpoint2.ContextId = ContextId.Local;
        Assert.AreNotEqual(endpoint, endpoint2);
        Assert.AreNotEqual(endpoint.GetHashCode(), endpoint2.GetHashCode());
        endpoint2.ContextId = ContextId.Host;
        endpoint2.Port = 42;
        Assert.AreNotEqual(endpoint, endpoint2);
        Assert.AreNotEqual(endpoint.GetHashCode(), endpoint2.GetHashCode());
        endpoint2.Port = 12345;
        endpoint2.ToHost = true;
        Assert.AreNotEqual(endpoint, endpoint2);
        Assert.AreNotEqual(endpoint.GetHashCode(), endpoint2.GetHashCode());

        Assert.IsFalse(endpoint.Equals(null));
        Assert.IsFalse(endpoint.Equals("foo"));
    }

    [TestMethod]
    public void TestToString()
    {
        if (!OperatingSystem.IsLinux())
        {
            Assert.Inconclusive("VSock sockets are only supported on Linux.");
            return;
        }

        var endpoint = new VSockEndPoint(ContextId.Host, 12345);
        Assert.AreEqual("vsock[2:12345]", endpoint.ToString());
    }
}

#endif
