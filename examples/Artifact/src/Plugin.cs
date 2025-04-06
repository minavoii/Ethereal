using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;

namespace Artifacts;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Aethermancer.exe")]
[BepInDependency("minavoii.ethereal")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    internal static readonly string ExamplesPath = Path.Join(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
        "Ethereal.Examples"
    );

    internal static readonly string EquipmentsPath = Path.Join(ExamplesPath, "Artifacts");

    private void Awake()
    {
        Logger = base.Logger;

        Ethereal.API.Artifacts.Add(
            Artifacts.TrucePact.descriptor,
            Artifacts.TrucePact.localisationData,
            Artifacts.TrucePact.customLanguageEntries
        );
    }
}
