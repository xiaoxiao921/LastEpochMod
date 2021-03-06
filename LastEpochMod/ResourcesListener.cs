using Harmony;
using MelonLoader;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LastEpochMod
{
    class ResourcesListener
    {
        [HarmonyPostfix]
        internal static void Postfix1(ref string path)
        {
            MelonLogger.Msg("Resources.Load Path : " + path);
        }

        [HarmonyPrefix]
        internal static bool Prefix2(ref Object __result, ref string path, ref Il2CppSystem.Type systemTypeInstance)
        {
            if (path.Contains("Headhunter"))
            {
                __result = Items.Headhunter.Sprite;

                return false;
            }

            return true;
        }

        [HarmonyPostfix]
        internal static void Postfix2(ref string path, ref Il2CppSystem.Type systemTypeInstance)
        {
            MelonLogger.Msg("Resources.Load Path : " + path + " | Type : " + systemTypeInstance.ToString());
        }

        internal static void Init()
        {
            var bf = (BindingFlags)(-1);

            var resourcesLoadMethod = typeof(Resources).GetMethods(bf).Where(m => m.Name == nameof(Resources.Load) && m.GetParameters().Length == 1).ToArray()[0];
            var postfix = new HarmonyMethod(typeof(ResourcesListener).GetMethod(nameof(Postfix1), bf));

            LastEpochMod.Instance.Harmony.Patch(resourcesLoadMethod, null, postfix);

            var resourcesLoadMethod2 = typeof(Resources).GetMethods(bf).Where(m => m.Name == nameof(Resources.Load) && m.GetParameters().Length == 2).ToArray()[0];
            var prefix2 = new HarmonyMethod(typeof(ResourcesListener).GetMethod(nameof(Prefix2), bf));
            var postfix2 = new HarmonyMethod(typeof(ResourcesListener).GetMethod(nameof(Postfix2), bf));

            LastEpochMod.Instance.Harmony.Patch(resourcesLoadMethod2, prefix2, postfix2);
        }
    }
}
