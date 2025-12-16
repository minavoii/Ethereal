using HarmonyLib;

namespace Ethereal.Patches;

internal static class Controller
{
    [HarmonyPatch(typeof(GameController), "Initialize")]
    [HarmonyPrefix]
    private static async void Prefix()
    {
        API.Referenceables.SetReady();
        API.Fonts.SetReady();
        API.Localisation.SetReady();
        API.MonsterTypes.SetReady();
        API.Perks.SetReady();
        API.Biomes.SetReady();
        API.Encounters.SetReady();
        await API.Sprites.SetReady();
    }

    [HarmonyPatch(typeof(GameController), "Initialize")]
    [HarmonyPostfix]
    private static void Postfix()
    {
        API.Buffs.SetReady();
        API.Elements.SetReady();
        API.Actions.SetReady();
        API.Traits.SetReady();
        API.Monsters.SetReady();
        API.Mementos.SetReady();
    }
}
