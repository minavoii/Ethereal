using System;
using System.Collections.Generic;
using System.Globalization;
using HarmonyLib;
using UnityEngine.UIElements.Collections;
using static Ethereal.API.Localisation;

namespace Ethereal.Patches;

internal static class Localisation
{
    [HarmonyPatch(typeof(Loca), nameof(Loca.Localize))]
    [HarmonyPrefix]
    private static bool Localize(string text, ref string __result)
    {
        string res = text;

        GameController.Instance.LocalisationData.LocaEntries.TryGetValue(
            text,
            out LocalisationData.LocalisationDataEntry value
        );

        if (value != null)
        {
            string localizedString =
                value.GetLocalizedString(GameSettingsController.Instance.CurrentLanguage)
                // Required if we get null values from uninitialized strings
                ?? string.Empty;

            if (localizedString != string.Empty)
                res = localizedString;
        }

        __result = res.Replace("\\n", "\n");
        return false;
    }

    [HarmonyPatch(typeof(Loca), nameof(Loca.GetLanguageString))]
    [HarmonyPrefix]
    private static bool GetLanguageString(ELanguage language, ref string __result)
    {
        __result = language switch
        {
            ELanguage.English => "English",
            ELanguage.French => "Français",
            ELanguage.German => "Deutsch",
            ELanguage.Italian => "Italiano",
            ELanguage.Russian => "Русский",
            ELanguage.Chinese => "中文",
            ELanguage.Japanese => "日本語",
            ELanguage.Spanish => "Español",
            ELanguage.Portuguese => "Português(BR)",
            ELanguage.Korean => "한국어",
            ELanguage custom => CustomLanguages.Get(custom) ?? "Unknown Language",
        };

        return false;
    }

    [HarmonyPatch(
        typeof(LocalisationData.LocalisationDataEntry),
        nameof(LocalisationData.LocalisationDataEntry.GetLocalizedString)
    )]
    [HarmonyPrefix]
    private static bool GetLocalizedString(
        ELanguage language,
        LocalisationData.LocalisationDataEntry __instance,
        ref string __result
    )
    {
        __result = language switch
        {
            ELanguage.English => __instance.StringContentEnglish,
            ELanguage.German => __instance.StringContentGerman,
            ELanguage.Spanish => __instance.StringContentSpanish,
            ELanguage.Portuguese => __instance.StringContentPortuguese,
            ELanguage.French => __instance.StringContentFrench,
            ELanguage.Italian => __instance.StringContentItalian,
            ELanguage.Russian => __instance.StringContentRussian,
            ELanguage.Chinese => __instance.StringContentSimplifiedChinese,
            ELanguage.Japanese => __instance.StringContentJapanese,
            ELanguage customLang => (
                CustomLocalisations.Get(__instance.StringContent ?? "")?.Data.Get(customLang)
            ) ?? __instance.StringContentEnglish,
        };

        return false;
    }

    [HarmonyPatch(typeof(global::Utils), "GetCultureInfo")]
    [HarmonyPrefix]
    private static bool GetCultureInfo(ref IFormatProvider __result)
    {
        var CultureInfos =
            (Dictionary<ELanguage, CultureInfo>)
                AccessTools.Field(typeof(global::Utils), "CultureInfos").GetValue(null);

        if (CultureInfos == null)
        {
            CultureInfos = new Dictionary<ELanguage, CultureInfo>
            {
                { ELanguage.English, CultureInfo.CreateSpecificCulture("en-US") },
                { ELanguage.Chinese, CultureInfo.CreateSpecificCulture("zh-CN") },
                { ELanguage.French, CultureInfo.CreateSpecificCulture("fr-FR") },
                { ELanguage.German, CultureInfo.CreateSpecificCulture("de-DE") },
                { ELanguage.Italian, CultureInfo.CreateSpecificCulture("it-IT") },
                { ELanguage.Russian, CultureInfo.CreateSpecificCulture("ru-RU") },
                { ELanguage.Spanish, CultureInfo.CreateSpecificCulture("es-ES") },
                { ELanguage.Japanese, CultureInfo.CreateSpecificCulture("ja-JP") },
            };

            AccessTools.Field(typeof(global::Utils), "CultureInfos").SetValue(null, CultureInfos);
        }

        __result = CultureInfos[GameSettingsController.Instance.CurrentLanguage];

        return false;
    }
}
