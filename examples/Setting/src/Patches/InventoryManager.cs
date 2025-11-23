using Ethereal.Classes.Settings;
using HarmonyLib;

namespace ExampleSetting.Patches;

internal static class InventoryManagerPatch
{
    [HarmonyPatch(typeof(InventoryManager), "RemoveSkillReroll")]
    [HarmonyPrefix]
    static bool RemoveSkillReroll()
    {
        // Don't run this if BuildTesterMode is on
        return !GameSettingsController.Instance.GetCustom<bool>("Build Tester Mode");
    }
}