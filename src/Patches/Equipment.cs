using HarmonyLib;

namespace Ethereal.Patches;

internal static class Equipment
{
    [HarmonyPatch(typeof(WorldData), nameof(WorldData.BuildReferenceablesCache))]
    [HarmonyPrefix]
    private static void BuildCache()
    {
        API.Equipment.ReadQueue();
    }
}
