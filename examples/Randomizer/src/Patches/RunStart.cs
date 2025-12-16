using HarmonyLib;

namespace Randomizer.Patches;

internal static class RunStart
{
    /// <summary>
    /// Runs before a new run starts, right before starter selection.
    /// </summary>
    [HarmonyPatch(typeof(NextAreaInteractable), "StartRun")]
    [HarmonyPrefix]
    private static async void Prefix()
    {
        await API.Randomizer.Randomize();
    }
}
