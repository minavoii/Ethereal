using BepInEx;
using BepInEx.Logging;
using Ethereal.API;
using Ethereal.Classes.Settings;

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

        Settings.AddTab(new("Custom"));
        Settings.AddSetting(new BooleanCustomSetting("Accessibility", "Test Accessibility", "This is a test accessibility boolean value", "Accessibility_test", false));
        Settings.AddSetting(new BooleanCustomSetting("Custom", "Test Custom", "This is a test boolean value on a new page", "Custom_test", true));
    }
}
