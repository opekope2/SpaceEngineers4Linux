using System.Drawing;
using System.Reflection;
using HarmonyLib;

namespace SpaceEngineers4Linux.RuntimePatches;

[HarmonyPatch]
public class SystemDrawingCommon_Region_Patches
{
    [HarmonyPatch(typeof(Region), "GetHrgn")]
    [HarmonyPrefix]
    static bool GetHrgn_Prefix(ref IntPtr __result, Region __instance, Graphics? g)
    {
        if (g != null) return true;

        __result = (IntPtr)typeof(Region).GetProperty("NativeRegion",
            BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(__instance)!;
        return false;
    }
}