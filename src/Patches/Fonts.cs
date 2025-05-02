using HarmonyLib;
using TMPro;

namespace Ethereal.Patches;

internal static class Fonts
{
    [HarmonyPrefix, HarmonyPatch(typeof(TMP_FontAsset), "Awake")]
    private static void Awake(TMP_FontAsset __instance)
    {
        API.Fonts.AddAllFallbacks(__instance);
    }
}
