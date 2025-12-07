using HarmonyLib;

namespace Ethereal.Patches;

internal static class DataTracking
{
    /// <summary>
    /// Prevent data tracking to avoid messing with official/vanilla data.
    /// </summary>
    /// <param name="__result"></param>
    /// <returns></returns>
    [HarmonyPatch(typeof(DataTracker), nameof(DataTracker.CanTrack))]
    [HarmonyPrefix]
    private static bool CanTrack(ref bool __result)
    {
        __result = false;
        return false;
    }
}
