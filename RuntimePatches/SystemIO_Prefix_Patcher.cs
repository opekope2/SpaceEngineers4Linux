using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MonoMod.Utils;

namespace SpaceEngineers4Linux.RuntimePatches;

partial class SystemIO_Patcher
{
    [HarmonyPatch]
    static class PrefixPatcher
    {
        static MethodInfo CreatePrefix(MethodBase method)
        {
            ParameterInfo[] parms = method.GetParameters();
            List<Type> parameters = new List<Type>();
            foreach (var parameter in parms)
            {
                if (parameter.ParameterType == typeof(string) && !parametersToExclude.Contains(parameter.Name))
                {
                    parameters.Add(typeof(string).MakeByRefType());
                }
            }

            DynamicMethodDefinition prefix = new DynamicMethodDefinition(
                $"{method.DeclaringType!.Name}_{method.Name}_Prefix_Generated", typeof(bool), parameters.ToArray());

            for (int i = 0, c = 0; i < parms.Length; i++)
            {
                if (parms[i].ParameterType == typeof(string) && !parametersToExclude.Contains(parms[i].Name))
                {
                    prefix.Definition.Parameters[c++].Name = parms[i].Name;
                }
            }

            MethodInfo stringReplace = typeof(string).GetMethod("Replace", new[] { typeof(char), typeof(char) })!;

            ILGenerator gen = prefix.GetILGenerator();

            for (int i = 0, c = prefix.Definition.Parameters.Count; i < c; i++)
            {
                gen.Emit(OpCodes.Ldarg, i);
                gen.Emit(OpCodes.Ldarg, i);
                gen.Emit(OpCodes.Ldind_Ref);
                gen.Emit(OpCodes.Ldc_I4, '\\');
                gen.Emit(OpCodes.Ldc_I4, '/');
                gen.Emit(OpCodes.Callvirt, stringReplace);
                gen.Emit(OpCodes.Stind_Ref);
            }

            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Ret);

            return prefix.Generate();
        }

        static MethodInfo Prefix(MethodBase m)
        {
            return CreatePrefix(m);
        }

        static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (Type type in typesToPatch)
            {
                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance |
                                                       BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    if (NeedsPrefix(method))
                    {
                        yield return method;
                    }
                }

                foreach (var ctor in type.GetConstructors(BindingFlags.Static | BindingFlags.Instance |
                                                          BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    if (NeedsPrefix(ctor))
                    {
                        yield return ctor;
                    }
                }
            }
        }
    }
}