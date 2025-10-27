using HarmonyLib;

namespace Ethereal.Patches;

internal static class World
{
    [HarmonyPatch(typeof(WorldData), nameof(WorldData.BuildReferenceablesCache))]
    [HarmonyPrefix]
    private static void BuildCache()
    {
        API.Artifacts.SetReady();
        API.Equipments.SetReady();
    }
}
