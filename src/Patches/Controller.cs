using HarmonyLib;

namespace Ethereal.Patches;

internal static class Controller
{
    [HarmonyPatch(typeof(GameController), "Initialize")]
    [HarmonyPrefix]
    private static void PostFix()
    {
        API.Localisation.GenerateTemplate();
        API.Localisation.LoadLanguages();
        API.Localisation.ReadQueue();

        API.Monster.ReadQueue();
    }
}
