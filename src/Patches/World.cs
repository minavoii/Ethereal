using HarmonyLib;

namespace Ethereal.Patches;

internal static class World
{
    [HarmonyPatch(typeof(WorldData), nameof(WorldData.BuildReferenceablesCache))]
    [HarmonyPrefix]
    private static void BuildCache()
    {
        API.Artifacts.ReadQueue();
        API.Equipments.ReadQueue();
        API.Mementos.ReadQueue();
    }
}
