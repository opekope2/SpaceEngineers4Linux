using System.Reflection;
using System.Windows.Forms;
using HarmonyLib;
using VRage;

namespace SpaceEngineers4Linux.RuntimePatches;

[HarmonyPatch]
public static class VRagePlatformWindows_MyWindowsWindows_Patches
{
    [HarmonyPrefix]
    static bool MessageBox_Prefix(ref VRage.MessageBoxResult __result, string text, string caption,
        VRage.MessageBoxOptions buttons)
    {
        __result = ConvertResultToVRage(MessageBox.Show(text, caption, GetButtonsFromOptions(buttons),
            GetIconFromOptions(buttons)));
        return false;
    }

    static MessageBoxButtons GetButtonsFromOptions(VRage.MessageBoxOptions options)
    {
        return options switch
        {
            VRage.MessageBoxOptions.OkOnly => MessageBoxButtons.OK,
            VRage.MessageBoxOptions.OkCancel => MessageBoxButtons.OKCancel,
            VRage.MessageBoxOptions.AbortRetryIgnore => MessageBoxButtons.AbortRetryIgnore,
            VRage.MessageBoxOptions.YesNoCancel => MessageBoxButtons.YesNoCancel,
            VRage.MessageBoxOptions.YesNo => MessageBoxButtons.YesNo,
            VRage.MessageBoxOptions.RetryCancel => MessageBoxButtons.RetryCancel,
            VRage.MessageBoxOptions.CancelTryContinue => MessageBoxButtons.YesNo,
            _ => MessageBoxButtons.OK
        };
    }

    static MessageBoxIcon GetIconFromOptions(VRage.MessageBoxOptions options)
    {
        return options switch
        {
            VRage.MessageBoxOptions.IconHand => MessageBoxIcon.Hand,
            VRage.MessageBoxOptions.IconQuestion => MessageBoxIcon.Question,
            VRage.MessageBoxOptions.IconExclamation => MessageBoxIcon.Exclamation,
            VRage.MessageBoxOptions.IconAsterisk => MessageBoxIcon.Asterisk,
            _ => MessageBoxIcon.None
        };
    }

    static VRage.MessageBoxResult ConvertResultToVRage(DialogResult result)
    {
        return result switch
        {
            DialogResult.Abort => MessageBoxResult.Abort,
            DialogResult.Cancel => MessageBoxResult.Cancel,
            DialogResult.Ignore => MessageBoxResult.Ignore,
            DialogResult.No => MessageBoxResult.No,
            DialogResult.OK => MessageBoxResult.Ok,
            DialogResult.Retry => MessageBoxResult.Retry,
            DialogResult.Yes => MessageBoxResult.Yes,
            _ => MessageBoxResult.Close
        };
    }

    static MethodBase TargetMethod()
    {
        return typeof(VRage.Platform.Windows.Sys.MySingleProgramInstance).Assembly
            .GetType("VRage.Platform.Windows.Forms.MyWindowsWindows")!.GetMethod("MessageBox",
                new[] { typeof(string), typeof(string), typeof(VRage.MessageBoxOptions) })!;
    }
}