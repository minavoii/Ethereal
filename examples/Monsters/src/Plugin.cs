using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Ethereal.API;
using ExampleMonsters.CustomActions;
using ExampleMonsters.CustomBuffs;
using ExampleMonsters.CustomMonsters;
using ExampleMonsters.CustomTraits;
using UnityEngine;

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

    private async void Awake()
    {
        Logger = base.Logger;

        // Some of Unity's methods don't work properly in async contexts,
        // so we initiliaze VFXs here instead of inside their constructors
        await APIManager.WhenReady(EtherealAPI.Buffs);

        Rotation.Buff.VFXApply = VFXs.CreateCosmetic(Rotation.Animation);
        WheelSupremacyAction.VFX.VFX = VFXs.CreateCosmetic(Rotation.Animation);
        ManyEyed.VFX.VFX = VFXs.CreateCosmetic(ManyEyed.Animation);
        TwistedGarden.VFX.VFX = VFXs.CreateCosmetic(TwistedGarden.Animation);

        if (await Buffs.Get("Age") is Buff age)
        {
            Gradient gradient = age.VFXApply.GetComponent<CosmeticVfxAnimator>().ColorOverLifetime;

            // Apply effects to the Rotation buff
            Rotation.Buff.SfxApply = age.SfxApply;
            Rotation.Buff.VFXApply.GetComponent<CosmeticVfxAnimator>().ColorOverLifetime = gradient;

            // Apply effects to Wheel Supremacy's action
            WheelSupremacyAction.VFX.VFX.GetComponent<CosmeticVfxAnimator>().ColorOverLifetime =
                gradient;
            WheelSupremacyAction.VFX.FlipX = true;
        }

        // Use another action's VFX
        if (await Actions.Get("Moisturize") is BaseAction moisturize)
        {
            FountainOfLife.Action.VFXs.Add(
                new() { VFX = moisturize.ActionVFX?.Children?.FirstOrDefault()?.VFX }
            );
        }

        await Keywords.Add(Rotation.Keyword);
        await Buffs.Add(Rotation.Buff);

        await Actions.Add(ManyEyed.Action, ManyEyed.Modifiers, true);
        await Actions.Add(TwistedGarden.Action, TwistedGarden.Modifiers);
        await Actions.Add(FountainOfLife.Action, FountainOfLife.Modifiers);

        await Actions.Add(WheelSupremacyAction.Action, WheelSupremacyAction.Modifiers);
        await Traits.Add(WheelSupremacy.Trait);

        await Monsters.Add(WaterWheel.Builder);
        await Mementos.Add(
            WaterWheel.Memento,
            WaterWheel.MementoShifted,
            WaterWheel.MetaUpgrade,
            "Special"
        );

        if (await Encounters.Get(EArea.PilgrimagePath, 3) is MonsterEncounterSet set)
        {
            MonsterEncounter wardenEncounter = set.MonsterEncounters.FirstOrDefault();

            await Encounters.SetEnemies(wardenEncounter, [new(WaterWheel.Builder.Monster.ID)]);
        }
    }
}
