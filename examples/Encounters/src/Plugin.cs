using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Ethereal.API;

namespace ExampleEncounters;

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

    private void Awake()
    {
        Logger = base.Logger;

        APIManager.RunWhenReady(
            EtherealAPI.Monsters | EtherealAPI.Encounters,
            () =>
            {
                Encounters.TryGet("MonsterEncounterSet_PP1_MB_0", out MonsterEncounterSet set);

                if (set != null)
                {
                    set.MonsterEncounters = [
                        new Encounters.EncounterDescriptor()
                        {
                            Type = MonsterEncounter.EEncounterType.Regular,
                            Enemies = ["Ammit", "Tatzelwurm"]
                        }.ToEncounter(),
                        new Encounters.EncounterDescriptor()
                        {
                            Type = MonsterEncounter.EEncounterType.Regular,
                            Enemies = ["Ammit", "Djinn"]
                        }.ToEncounter(),
                        new Encounters.EncounterDescriptor()
                        {
                            Type = MonsterEncounter.EEncounterType.Regular,
                            Enemies = ["Ammit", "Tatzelwurm"]
                        }.ToEncounter()
                    ];
                }
            }
        );
    }
}
