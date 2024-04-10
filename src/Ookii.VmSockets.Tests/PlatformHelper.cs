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
    [SupportedOSPlatformGuard("windows")]
#endif
    public static bool IsWindows()
    {
#if NET8_0_OR_GREATER
        return OperatingSystem.IsWindows();
#else
        return Environment.OSVersion.Platform == PlatformID.Win32NT;
#endif
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
