using BepInEx;
using BepInEx.Logging;
using Ethereal.API;
using Ethereal.Classes.Settings;
using HarmonyLib;

namespace ExampleSetting;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Aethermancer.exe")]
[BepInDependency("minavoii.ethereal")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;

        Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);

        harmony.PatchAll(typeof(Patches.InventoryManagerPatch));
        harmony.PatchAll(typeof(Patches.MonsterShrineMenuPatch));
        harmony.PatchAll(typeof(Patches.MonsterShrineTriggerPatch));
        harmony.PatchAll(typeof(Patches.SkillSelectMenuPatch));

        Settings.AddTab(new("Custom"));
        Settings.AddTab(new("Custom2"));

        Settings.AddSetting(
            new BooleanCustomSetting("Accessibility", "Build Tester Mode", "Turns on Build Testing mode; All available monsters in Monster Shrines, Infinite skill rerolls", "Accessibility_build_tester_mode", false)
            {
                IsEnabled = () => GameStateManager.Instance.IsMainMenu || ExplorationController.Instance.CurrentArea == EArea.PilgrimsRest
            }
        );

        Settings.AddSetting(new BooleanCustomSetting("Custom", "Test Custom", "This is a test boolean value on a new page", "Custom_test", true));
        Settings.AddSetting(new BooleanCustomSetting("Custom", "Test Custom2", "This is a test boolean value to show position is automatic", "Custom_test_2", false));
        Settings.AddSetting(
            new BooleanCustomSetting("Custom", "Test Very Long name with width override", "This is a test value to show you can override the width of the box", "Custom_test_4", false)
            {
                WidthOverride = 230
            });
        Settings.AddSetting(
            new BooleanCustomSetting("Custom", "Test Position Override", "This shows you can override the position of the setting. May not play well with auto-positioning", "Custom_test_5", false)
            {
                PositionOverrideX = 250,
                PositionOverrideY = 0
            });
        Settings.AddSetting(
            new SelectCustomSetting<string>("Custom2", "Test Select", "This is a test select option", "Custom2_select", [
                new("Option 1", "value1"),
                new("Option 2", "value2"),
                new("Option 3", "value3")
            ]));
    }
}
