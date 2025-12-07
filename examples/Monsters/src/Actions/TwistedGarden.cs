using System.Collections.Generic;
using System.IO;
using Ethereal.API;
using Ethereal.Classes.Builders;
using Ethereal.Classes.Wrappers;
using UnityEngine;

namespace ExampleMonsters.CustomActions;

internal static class TwistedGarden
{
    private const int ID = 1902;

    private const string Name = "Twisted Garden";

    private static readonly AnimationClip Animation = Animations.LoadFromAsset(
        Path.Join(Plugin.CustomMonstersPath, "TwistedGarden"),
        "assets/animations/actions/twistedgarden.prefab"
    );

    private static readonly VFX.ChildVFX VFX = new()
    {
        VFX = VFXs.CreateCosmetic(Animation),
        SpawnForEveryEnemy = true,
    };

    internal static readonly BaseActionBuilder Action = new(
        ID: ID,
        Name: Name,
        Cost: new(2, 0, 0, 2),
        ActionType: EActionType.MassTarget,
        TargetType: ETargetType.AllEnemies,
        SkillType: ESkillType.Attacker,
        Types: [],
        Elements: [EElement.Fire],
        AnimationType: EActionAnimationType.CastSpell,
        ActionIconBig: Sprites.LoadFromImage(
            Sprites.SpriteType.Action,
            Path.Join(Plugin.CustomMonstersPath, "ActionTwistedGarden_Large.png")
        ),
        ActionIconSmall: Sprites.LoadFromImage(
            Sprites.SpriteType.ActionSmall,
            Path.Join(Plugin.CustomMonstersPath, "ActionTwistedGarden_Small.png")
        ),
        ActionIconCutSmall: Sprites.LoadFromImage(
            Sprites.SpriteType.ActionCutSmall,
            Path.Join(Plugin.CustomMonstersPath, "ActionTwistedGarden_SmallCut.png")
        ),
        AnimationBuildupTime: 0.6f,
        VFXs: [VFX]
    );

    internal static List<ActionModifier> Modifiers => [Damage, ApplyBuff];

    private static readonly ActionDamage Damage = new()
    {
        Damage = 4,
        HitCount = 3,
        BetweenHitsDelay = 0.2f,
    };

    private static readonly ActionApplyBuffWrapper ApplyBuff = new([new("Terror", 10)])
    {
        ApplyType = ActionApplyBuff.EApplyType.AllListedBuffs,
        ApplyDelay = 1f,
    };
}
