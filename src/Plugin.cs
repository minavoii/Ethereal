using System.IO;
using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace Ethereal;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Aethermancer.exe")]
internal class Plugin : BaseUnityPlugin
{
    internal static readonly string PLUGINS_PATH = Path.GetDirectoryName(
        Assembly.GetExecutingAssembly().Location
    );

    internal static readonly string ETHEREAL_PATH = Path.Join(PLUGINS_PATH, "ethereal");

    private void Awake()
    {
        Log.Plugin = Logger;

        Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);

        harmony.PatchAll(typeof(Patches.Controller));
        harmony.PatchAll(typeof(Patches.Localisation));
        harmony.PatchAll(typeof(Patches.Equipment));
    }
}
