using Ethereal.API;
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
        if (Perks.TryGetAll(out var perks))
            API.Data.AllPerks = perks;

        if (Traits.TryGetAllSignature(out var signatureTraits))
            API.Data.SignatureTraits = signatureTraits;

        API.Randomizer.BalanceChanges();
        API.Randomizer.LoadData();
    }
}
