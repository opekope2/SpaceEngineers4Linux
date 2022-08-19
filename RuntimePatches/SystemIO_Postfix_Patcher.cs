using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MonoMod.Utils;

namespace SpaceEngineers4Linux.RuntimePatches;

partial class SystemIO_Patcher
{
    //[HarmonyPatch]
    static class PostfixPatcher
    {
        static MethodInfo CreatePostfix(MethodBase method)
        {
            DynamicMethodDefinition postfix = new DynamicMethodDefinition(
                $"{method.DeclaringType!.Name}_{method.Name}_Postfix_Generated", typeof(void),
                new[] { typeof(string).MakeByRefType() });
            postfix.Definition.Parameters[0].Name = "__result";

            MethodInfo stringReplace = typeof(string).GetMethod("Replace", new[] { typeof(char), typeof(char) })!;

            ILGenerator gen = postfix.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldind_Ref);
            gen.Emit(OpCodes.Ldc_I4, '/');
            gen.Emit(OpCodes.Ldc_I4, '\\');
            gen.Emit(OpCodes.Callvirt, stringReplace);
            gen.Emit(OpCodes.Stind_Ref);
            gen.Emit(OpCodes.Ret);

            return postfix.Generate();
        }

        static MethodInfo Postfix(MethodBase m)
        {
            return CreatePostfix(m);
        }

        static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (Type type in typesToPatch)
            {
                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance |
                                                       BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    if (NeedsPostfix(method))
                    {
                        yield return method;
                    }
                }
            }
        }
    }
}