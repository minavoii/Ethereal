using HarmonyLib;

namespace Ethereal.Patches;

public class Controller
{
    [HarmonyPatch(typeof(GameController), "Initialize")]
    [HarmonyPrefix]
    private static void PostFix()
    {
        API.Localisation.GenerateTemplate();
        API.Localisation.LoadLanguages();
    }
}
