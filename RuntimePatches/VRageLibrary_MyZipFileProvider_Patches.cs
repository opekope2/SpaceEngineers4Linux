using HarmonyLib;
using VRage.FileSystem;

namespace SpaceEngineers4Linux.RuntimePatches;

[HarmonyPatch]
public static class VRageLibrary_MyZipFileProvider_Patches
{
    [HarmonyPatch(typeof(MyZipFileProvider), MethodType.Constructor)]
    [HarmonyPostfix]
    static void MyZipFileProvider_Ctor_Postfix(ref MyZipFileProvider __instance)
    {
        //__instance.GetType().GetField("Separators").SetValue(__instance, new char[] { '\\', '/' });
    }

    public static void Load_VRageLibrary()
    {
        Type _ = typeof(MyZipFileProvider);
    }
}
