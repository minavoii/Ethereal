
using Ethereal.Classes.Settings;
using HarmonyLib;
using UnityEngine;

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

    [HarmonyPatch(typeof(SettingsMenu), "Open")]
    [HarmonyPrefix]
    static void Open()
    {
        foreach (var customSetting in API.Settings.CustomSettings)
        {
            customSetting.UpdateControlState();
        }
    }

    [HarmonyPatch(typeof(SettingsMenu), "CreateSettingsSnapshot")]
    [HarmonyPostfix]
    static void CreateSettingsSnapshot(SettingsMenu __instance)
    {
        SettingsSnapshot snapshot = Traverse.Create(__instance)
                .Field("rollBackSnapshot")
                .GetValue<SettingsSnapshot>();

        if (snapshot == null)
            return;

        var extra = snapshot.Extra();

        foreach (var customSetting in API.Settings.CustomSettings)
        {
            customSetting.SetRollbackSnapshot(extra);
        }
    }

    [HarmonyPatch(typeof(SettingsMenu), "ApplySnapshot")]
    [HarmonyPostfix]
    static void ApplySnapshot(SettingsMenu __instance, SettingsSnapshot snapshot)
    {
        int currentIndex = Traverse.Create(__instance)
                .Field("currentPageIndex")
                .GetValue<int>();

        foreach (var customSetting in API.Settings.CustomSettings)
        {
            int pageIndex = API.Settings.Tabs.FindIndex((page) => page == customSetting.Tab);
            if (pageIndex == -1)
            {
                Debug.LogError($"Custom setting {customSetting.Name} on invalid tab {customSetting.Tab}");
                continue;
            }
            if (pageIndex != currentIndex)
            {
                continue;
            }

            customSetting.ApplySnapshot(snapshot.Extra());
        }
    }
}
