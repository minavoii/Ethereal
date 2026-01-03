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
        Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

        harmony.PatchAll(typeof(Patches.Controller));
        harmony.PatchAll(typeof(Patches.Fonts));
        harmony.PatchAll(typeof(Patches.Localisation));
        harmony.PatchAll(typeof(Patches.World));
        harmony.PatchAll(typeof(Patches.Action));
        harmony.PatchAll(typeof(Patches.PilgrimsRest));
        harmony.PatchAll(typeof(Patches.MonsterState));
        harmony.PatchAll(typeof(Patches.CombatState));
        harmony.PatchAll(typeof(Patches.Projectiles));
        harmony.PatchAll(typeof(Patches.MonsterInit));
        harmony.PatchAll(typeof(Patches.ShiftedDisplay));
    }
}
