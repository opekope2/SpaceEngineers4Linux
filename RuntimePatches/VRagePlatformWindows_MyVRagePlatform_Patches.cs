using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace SpaceEngineers4Linux.RuntimePatches;

[HarmonyPatch]
public static class VRagePlatformWindows_MyVRagePlatform_Patches
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Ctor_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instr in instructions)
        {
            if (instr.opcode == OpCodes.Stsfld && instr.operand is FieldInfo { Name: "ENABLE_TIMING_HOTFIX" })
            {
                yield return new CodeInstruction(OpCodes.Pop);
            }
            else
            {
                yield return instr;
            }
        }
    }

    static MethodBase TargetMethod()
    {
        return typeof(VRage.Platform.Windows.Sys.MySingleProgramInstance).Assembly
            .GetType("VRage.Platform.Windows.MyVRagePlatform")!.GetConstructors()[0];
    }
}