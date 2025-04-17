using HarmonyLib;

namespace Ethereal.Patches;

internal static class Action
{
    [HarmonyPatch(typeof(BaseAction), "InitializeElements")]
    [HarmonyPrefix]
    private static void InitializeElements(BaseAction __instance)
    {
        __instance.ElementsOverride ??= [];
        __instance.Cost ??= new();
    }
}
