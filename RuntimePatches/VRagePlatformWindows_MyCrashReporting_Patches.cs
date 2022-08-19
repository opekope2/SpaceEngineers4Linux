using HarmonyLib;

namespace SpaceEngineers4Linux.RuntimePatches;

[HarmonyPatch]
static class VRagePlatformWindows_MyCrashReporting_Patches
{
    [HarmonyPatch("VRage.Platform.Windows.MyCrashReporting", "WriteMiniDump")]
    [HarmonyPrefix]
    static bool WriteMiniDump_Prefix()
    {
        Console.WriteLine("Writing minidump is not supported");
        return false;
    }
}
