using System.Reflection.Emit;
using HarmonyLib;

namespace SpaceEngineers4Linux.RuntimePatches;

[HarmonyPatch]
public class SystemWindowsForms_X11Keyboard_Patches
{
    [HarmonyPatch("System.Windows.Forms.X11Keyboard", "CreateXic")]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> CreateXic_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        bool skip = true;
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Endfinally)
            {
                skip = false;
                yield return new CodeInstruction(OpCodes.Ldsfld, typeof(IntPtr).GetField("Zero"));
                yield return new CodeInstruction(OpCodes.Stloc_0);
                continue;
            }

            if (!skip)
            {
                yield return instruction;
            }
        }
    }
}