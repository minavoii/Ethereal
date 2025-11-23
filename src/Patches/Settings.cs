
using HarmonyLib;

namespace Ethereal.Patches;

internal static class Settings
{
    [HarmonyPatch(typeof(SettingsMenu), "Awake")]
    [HarmonyPrefix]
    static void Awake_Prefix()
    {
        API.Settings.SetReady();
    }

    [HarmonyPatch(typeof(SettingsMenu), "Awake")]
    [HarmonyPostfix]
    static void Awake_Postfix()
    {
        API.Settings.FixTabHeaders();
    }
}
