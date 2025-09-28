using HarmonyLib;
using TMPro;

namespace Ethereal.Patches;

internal static class Fonts
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(FontMapper), nameof(FontMapper.SetCurrentLanguage))]
    private static void SetCurrentLanguage(FontMapper __instance)
    {
        foreach (FontMapper.FontMapping mapping in __instance.FontMappings)
        {
            foreach (TMP_FontAsset fontAsset in API.Fonts.CustomFonts)
            {
                fontAsset.material = mapping.MainFont.material;
                mapping.MainFont.fallbackFontAssetTable.Add(fontAsset);
            }
        }
    }
}
