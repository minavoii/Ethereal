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
    private static async void Prefix()
    {
        API.Data.AllPerks = await API.Data.GetAllPerkInfos();
        API.Data.SignatureTraits = await Traits.GetAllSignature();

        await API.Randomizer.BalanceChanges();
        await API.Randomizer.LoadData();
    }
}
