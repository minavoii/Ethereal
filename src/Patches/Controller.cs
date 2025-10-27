using HarmonyLib;

namespace Ethereal.Patches;

internal static class Controller
{
    [HarmonyPatch(typeof(GameController), "Initialize")]
    [HarmonyPrefix]
    private static void Prefix()
    {
        API.Referenceables.SetReady();
        API.Fonts.SetReady();
        API.Localisation.SetReady();
        API.MonsterTypes.SetReady();
        API.Actions.SetReady();
        API.Traits.SetReady();
        API.Perks.SetReady();
        API.Monsters.SetReady();
        API.Sprites.SetReady();
    }

    [HarmonyPatch(typeof(GameController), "Initialize")]
    [HarmonyPostfix]
    private static void Postfix()
    {
        API.Buffs.SetReady();
        API.Elements.SetReady();
    }
}
