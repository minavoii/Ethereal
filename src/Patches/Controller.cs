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

        API.Monster.ReadQueue();
        API.Trait.ReadQueue();
        API.Action.ReadQueue();
        API.MonsterTypes.ReadQueue();
    }

    [HarmonyPatch(typeof(GameController), "Initialize")]
    [HarmonyPostfix]
    private static void Postfix()
    {
        API.Buffs.ReadQueue();
    }
}
