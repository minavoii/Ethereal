using HarmonyLib;

namespace Randomizer.Patches;

internal static class RunStart
{
    /// <summary>
    /// Runs before a new run starts and the difficulty selector appears.
    /// </summary>
    [HarmonyPatch(typeof(NextAreaInteractable), "StartInteraction")]
    [HarmonyPrefix]
    private static void Prefix()
    {
        API.Randomizer.Randomize();
    }
}
