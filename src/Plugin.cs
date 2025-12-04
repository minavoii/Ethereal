using System.IO;
using System.Reflection;
using BepInEx;
using Ethereal.API;
using HarmonyLib;

namespace Ethereal;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Aethermancer.exe")]
internal class Plugin : BaseUnityPlugin
{
    private Harmony? _harmony;

    private static readonly string PluginsPath = Path.GetDirectoryName(
        Assembly.GetExecutingAssembly().Location
    );

    internal static readonly string EtherealPath = Path.Join(PluginsPath, "Ethereal");

    private void Awake()
    {
        Log.Plugin = Logger;
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

        _harmony.PatchAll(typeof(Patches.Controller));
        _harmony.PatchAll(typeof(Patches.Fonts));
        _harmony.PatchAll(typeof(Patches.Localisation));
        _harmony.PatchAll(typeof(Patches.World));
        _harmony.PatchAll(typeof(Patches.Action));
        _harmony.PatchAll(typeof(Patches.PilgrimsRest));
        _harmony.PatchAll(typeof(Patches.MonsterState));
        _harmony.PatchAll(typeof(Patches.CombatState));
        _harmony.PatchAll(typeof(Patches.Projectiles));
        _harmony.PatchAll(typeof(Patches.MonsterInit));
    }


    private void OnDestroy()
    {
        APIManager.CleanupAPIs();
        if (_harmony != null)
        {
            _harmony.UnpatchSelf();
        }
    }
}
