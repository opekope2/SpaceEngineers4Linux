using System.Reflection;

namespace SpaceEngineers4Linux.RuntimePatches;

[HarmonyLib.HarmonyPatch]
static partial class SystemIO_Patcher
{
    static readonly Type[] typesToPatch = {
        typeof(Directory),
        typeof(DirectoryInfo),
        typeof(File),
        typeof(FileInfo),
        typeof(FileStream),
        typeof(FileSystemInfo),
        typeof(FileSystemWatcher),
        typeof(Path),
        typeof(StreamReader),
        //typeof(StreamWriter),
    };

    static readonly string[] parametersToExclude = { };

    static bool NeedsPrefix(MethodBase method)
    {
        return method.GetParameters().Any(p => p.ParameterType == typeof(string));
    }

    static bool NeedsPostfix(MethodInfo method)
    {
        return method.ReturnType == typeof(string);
    }
}
