using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Randomizer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Aethermancer.exe")]
[BepInDependency("minavoii.ethereal")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger = null!;

    private static readonly string PluginsPath = Path.GetDirectoryName(
        Assembly.GetExecutingAssembly().Location
    );

    internal static readonly string RandomizerPath = Path.Join(PluginsPath, "Minavoii.Randomizer");

    private void Awake()
    {
        Logger = base.Logger;

        Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);

        harmony.PatchAll(typeof(Patches.LoadGame));
        harmony.PatchAll(typeof(Patches.RunStart));
    }
}
