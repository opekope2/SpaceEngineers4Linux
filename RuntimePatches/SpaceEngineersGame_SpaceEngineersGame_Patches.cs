using HarmonyLib;
using SpaceEngineers.Game;

namespace SpaceEngineers4Linux.RuntimePatches;

[HarmonyPatch]
static class SpaceEngineersGame_SpaceEngineersGame_Patches
{
    [HarmonyPatch(typeof(SpaceEngineersGame), "SetupAnalytics")]
    [HarmonyPrefix]
    static bool SetupAnalytics_Prefix()
    {
        return false;
    }
}