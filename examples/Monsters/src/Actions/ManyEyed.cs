using System.Collections.Generic;
using System.IO;
using Ethereal.API;
using Ethereal.Classes.Builders;
using Ethereal.Classes.Wrappers;
using UnityEngine;

namespace ExampleMonsters.CustomActions;

internal static class ManyEyed
{
    private static readonly AnimationClip Animation = Animations.LoadFromAsset(
        Path.Join(Plugin.CustomMonstersPath, "ManyEyed"),
        "assets/animations/actions/manyeyed.prefab"
    );

    private static readonly VFX.ChildVFX VFX = new()
    {
        VFX = VFXs.CreateCosmetic(Animation),
        SpawnForEveryEnemy = true,
    };

    internal static readonly BaseActionBuilder Action = new(
        ID: 1904,
        Name: "Many Eyed",
        Cost: new(EElement.Fire, 1),
        ActionType: EActionType.SingleTarget,
        TargetType: ETargetType.SingleEnemy,
        SkillType: ESkillType.Attacker,
        Types: [],
        Elements: [EElement.Fire],
        AnimationType: EActionAnimationType.CastSpell,
        ActionIconBig: Sprites.LoadFromImage(
            Sprites.SpriteType.Action,
            Path.Join(Plugin.CustomMonstersPath, "ActionManyEyed_Large.png")
        ),
        ActionIconSmall: Sprites.LoadFromImage(
            Sprites.SpriteType.ActionSmall,
            Path.Join(Plugin.CustomMonstersPath, "ActionManyEyed_Small.png")
        ),
        ActionIconCutSmall: Sprites.LoadFromImage(
            Sprites.SpriteType.ActionCutSmall,
            Path.Join(Plugin.CustomMonstersPath, "ActionManyEyed_SmallCut.png")
        ),
        AnimationBuildupTime: 1.1f,
        VFXs: [VFX]
    );

    internal static List<ActionModifier> Modifiers => [Damage, ApplyBuff];

    private static readonly ActionDamage Damage = new()
    {
        HitCount = 3,
        Damage = 3,
        BetweenHitsDelay = 0.2f,
    };

    private static readonly ActionApplyBuffWrapper ApplyBuff = new([new("Terror", 3)])
    {
        ApplyType = ActionApplyBuff.EApplyType.AllListedBuffs,
        ApplyDelay = 0.6f,
    };
}
