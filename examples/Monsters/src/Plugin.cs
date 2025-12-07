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

    private void Awake()
    {
        Logger = base.Logger;

        APIManager.RunWhenReady(
            EtherealAPI.Buffs | EtherealAPI.Actions | EtherealAPI.Monsters,
            () =>
            {
                if (Buffs.TryGet("Age", out Buff age))
                {
                    Gradient gradient = age
                        .VFXApply.GetComponent<CosmeticVfxAnimator>()
                        .ColorOverLifetime;

                    // Apply effects to the Rotation buff
                    Rotation.Buff.SfxApply = age.SfxApply;
                    Rotation.Buff.VFXApply.GetComponent<CosmeticVfxAnimator>().ColorOverLifetime =
                        gradient;

                    // Apply effects to Wheel Supremacy's action
                    WheelSupremacyAction
                        .VFX.VFX.GetComponent<CosmeticVfxAnimator>()
                        .ColorOverLifetime = gradient;
                    WheelSupremacyAction.VFX.FlipX = true;
                }

                // Use another action's VFX
                if (Actions.TryGet("Moisturize", out BaseAction moisturize))
                {
                    FountainOfLife.Action.VFXs.Add(
                        new() { VFX = moisturize.ActionVFX?.Children?.FirstOrDefault()?.VFX }
                    );
                }

                Keywords.Add(Rotation.Keyword);
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
        );
    }
}
