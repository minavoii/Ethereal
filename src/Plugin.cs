using System.IO;
using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace Ethereal;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Aethermancer.exe")]
internal class Plugin : BaseUnityPlugin
{
    private static readonly string PluginsPath = Path.GetDirectoryName(
        Assembly.GetExecutingAssembly().Location
    );

    internal static readonly string EtherealPath = Path.Join(PluginsPath, "Ethereal");

    private void Awake()
    {
        Log.Plugin = Logger;

        Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);

        harmony.PatchAll(typeof(Patches.Controller));
        harmony.PatchAll(typeof(Patches.Localisation));
        harmony.PatchAll(typeof(Patches.Equipment));
    }
}
