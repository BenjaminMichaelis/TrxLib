using System.Runtime.InteropServices;

using TUnit.Core;

namespace TrxLib.Tests;

public sealed class WindowsOnlyAttribute() : SkipAttribute("This test is only supported on Windows")
{
    public override Task<bool> ShouldSkip(TestRegisteredContext context)
    {
        return Task.FromResult(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
    }
}

public sealed class NonWindowsOnlyAttribute() : SkipAttribute("This test is only supported on non-Windows platforms")
{
    public override Task<bool> ShouldSkip(TestRegisteredContext context)
    {
        return Task.FromResult(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
    }
}
