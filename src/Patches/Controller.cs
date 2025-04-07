using HarmonyLib;

namespace Ethereal.Patches;

internal static class Controller
{
    [HarmonyPatch(typeof(GameController), "Initialize")]
    [HarmonyPrefix]
    private static void Prefix()
    {
        API.Localisation.GenerateTemplate();
        API.Localisation.LoadLanguages();
        API.Localisation.ReadQueue();

        API.Monsters.ReadQueue();
        API.Traits.ReadQueue();
        API.Actions.ReadQueue();
        API.MonsterTypes.ReadQueue();
    }

    [HarmonyPatch(typeof(GameController), "Initialize")]
    [HarmonyPostfix]
    private static void Postfix()
    {
        API.Buffs.ReadQueue();
        API.Elements.ReadQueue();
    }
}
