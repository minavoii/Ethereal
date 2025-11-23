using Ethereal.Classes.Settings;
using HarmonyLib;

namespace ExampleSetting.Patches;

internal static class SkillSelectMenuPatch
{
    [HarmonyPatch(typeof(SkillSelectMenu), "Open")]
    [HarmonyPostfix]
    static void Open(SkillSelectMenu __instance)
    {
        if (GameSettingsController.Instance.GetCustom<bool>("Build Tester Mode"))
        {
            __instance.RerollSkillsButton.Text.text = Loca.TEXT_FORMAT("Reroll Skills ({0})", "∞");
            __instance.RerollSkillsButton.SetDisabled(false);
        }
    }
}