using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Ookii.VmSockets.Tests;

public static class PlatformHelper
{
#if NET8_0_OR_GREATER
    [SupportedOSPlatformGuard("windows10.0")]
#endif
    public static bool IsWindows10OrLater() => IsWindowsVersionAtLeast(10);

#if NET8_0_OR_GREATER
    [SupportedOSPlatformGuard("windows10.0.14393")]
#endif
    public static bool IsWindows10_1607OrLater() => IsWindowsVersionAtLeast(10, 0, 14393);

#if NET8_0_OR_GREATER
    [SupportedOSPlatformGuard("windows10.0.16299")]
#endif
    public static bool IsWindows10_1709OrLater() => IsWindowsVersionAtLeast(10, 0, 16299);

#if NET8_0_OR_GREATER
    [SupportedOSPlatformGuard("windows10.0.22621")]
#endif
    public static bool IsWindows11_22h2OrLater() => IsWindowsVersionAtLeast(10, 0, 22621);

    private static bool IsWindowsVersionAtLeast(int major, int minor = 0, int build = 0, int revision = 0)
    {
#if NET8_0_OR_GREATER
        if (OperatingSystem.IsWindowsVersionAtLeast(major, minor, build, revision))
#else
        if (Environment.OSVersion.Platform == PlatformID.Win32NT && 
            Environment.OSVersion.Version >= new Version(major, minor, build, revision))
#endif
        {
            return true;
        }

        Assert.Inconclusive($"This test is only supported on Windows version {major}.{minor}.{build}.{revision} and later.");
        return false;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct sockaddr_vm
    {
        [FieldOffset(0)]
        public short svm_family;
        [FieldOffset(2)]
        public short svm_reserved;
        [FieldOffset(4)]
        public int svm_port;
        [FieldOffset(8)]
        public int svm_cid;
        [FieldOffset(12)]
        public byte svm_flags;
        [FieldOffset(13)]
        public char svm_zero0;
        [FieldOffset(14)]
        public char svm_zero1;
        [FieldOffset(15)]
        public char svm_zero2;
    }
}
