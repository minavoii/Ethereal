using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Ethereal.API;
using ExampleMonsters.CustomActions;
using ExampleMonsters.CustomBuffs;
using ExampleMonsters.CustomMonsters;
using ExampleMonsters.CustomTraits;

namespace ExampleMonsters;

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

    internal static readonly string CustomMonstersPath = Path.Join(ExamplesPath, "Monsters");

    private void Awake()
    {
        Logger = base.Logger;

        Buffs.Add(Rotation.Buff);

        Actions.Add(ManyEyed.Action, ManyEyed.Modifiers, true);
        Actions.Add(TwistedGarden.Action, TwistedGarden.Modifiers);
        Actions.Add(FountainOfLife.Action, FountainOfLife.Modifiers);

        Actions.Add(WheelSupremacyAction.Action, WheelSupremacyAction.Modifiers);
        Traits.Add(WheelSupremacy.Trait);

        Monsters.Add(WaterWheel.Builder);
        Mementos.Add(
            WaterWheel.Memento,
            WaterWheel.MementoShifted,
            WaterWheel.MetaUpgrade,
            "Special"
        );
    }
}
