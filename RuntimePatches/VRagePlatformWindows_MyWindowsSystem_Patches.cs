using System.Diagnostics;
using HarmonyLib;

namespace SpaceEngineers4Linux.RuntimePatches;

[HarmonyPatch]
public static class VRagePlatformWindows_MyWindowsSystem_Patches
{
    static string? cpu = null;
    static uint cpuFreq = 0;

    [HarmonyPatch("VRage.Platform.Windows.Sys.MyWindowsSystem", "ProcessPrivateMemory", MethodType.Getter)]
    [HarmonyPrefix]
    static bool ProcessPrivateMemory_Prefix(ref long __result)
    {
        __result = Process.GetCurrentProcess().PrivateMemorySize64;
        return false;
    }

    [HarmonyPatch("VRage.Platform.Windows.Sys.MyWindowsSystem", "GetOsName")]
    [HarmonyPrefix]
    static bool GetOsName_Prefix(ref string __result)
    {
        __result = Native.GetOsName();
        return false;
    }

    [HarmonyPatch("VRage.Platform.Windows.Sys.MyWindowsSystem", "GetInfoCPU")]
    [HarmonyPrefix]
    static bool GetInfoCPU_Prefix(ref string __result, out uint frequency, out uint physicalCores)
    {
        if (cpu == null)
        {
            cpu = "UnknownCPU";
            try
            {
                foreach (string line in File.ReadAllLines("/proc/cpuinfo"))
                {
                    if (!line.StartsWith("model name")) continue;
                    cpu = line.Split(':')[1].Trim();
                    break;
                }
                cpuFreq = uint.Parse(File.ReadAllText("/sys/devices/system/cpu/cpu0/cpufreq/scaling_max_freq")) / 1000;
            }
            catch
            {
                // ignored
            }
        }

        frequency = cpuFreq;
        physicalCores = (uint)Environment.ProcessorCount;
        __result = cpu;
        return false;
    }

    /*[HarmonyPatch("VRage.Platform.Windows.Sys.MyWindowsSystem", "LogEnvironmentInformation")]
    [HarmonyPrefix]
    static bool LogEnvironmentInformation_Prefix()
    {
        // TODO
        return true;
    }

    [HarmonyPatch("VRage.Platform.Windows.Sys.MyWindowsSystem", "GetVideoDriverDetails")]
    [HarmonyPrefix]
    static bool GetVideoDriverDetails_Prefix()
    {
        // TODO. This will crash
        return true;
    }*/

    public static void Load_VRagePlatformWindows()
    {
        Type _ = typeof(VRage.Platform.Windows.Sys.MySingleProgramInstance);
    }
}
