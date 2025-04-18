using HarmonyLib;

namespace Randomizer.Patches;

internal static class LoadGame
{
    /// <summary>
    /// Runs when a save file is selected.
    /// </summary>
    [HarmonyPatch(typeof(SaveSlotMenu), "LoadExistingGame")]
    [HarmonyPrefix]
    private static void Prefix()
    {
        API.Data.AllPerks = API.Data.GetAllPerks();
        API.Data.SignatureTraits = API.Data.GetAllSignatureTraits();
        API.Randomizer.BalanceChanges();
        API.Randomizer.LoadData();
    }
}
