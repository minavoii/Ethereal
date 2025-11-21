using HarmonyLib;

namespace Ethereal.Patches;

internal static class Keyword
{
    [HarmonyPatch(typeof(KeywordManager), "Awake")]
    [HarmonyPostfix]
    private static void Postfix()
    {
        API.Keywords.SetReady();
    }
}
