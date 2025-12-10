using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Ethereal.API;

namespace ExampleArtifacts;

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

    internal static readonly string ArtifactsPath = Path.Join(ExamplesPath, "Artifacts");

    private void Awake()
    {
        Logger = base.Logger;

        Artifacts.Add(Artifact.TrucePact.Builder);
    }
}
