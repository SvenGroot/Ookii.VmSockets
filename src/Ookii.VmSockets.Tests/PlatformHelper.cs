using System;
using System.Collections.Generic;
using System.Linq;
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
}
