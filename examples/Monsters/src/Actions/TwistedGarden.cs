using System.Collections.Generic;
using System.IO;
using Ethereal.API;
using Ethereal.Classes.Builders;
using Ethereal.Classes.Wrappers;

namespace ExampleMonsters.CustomActions;

internal static class TwistedGarden
{
    internal static readonly BaseActionBuilder Action = new(
        ID: 1902,
        Name: "Twisted Garden",
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
        AnimationBuildupTime: 0.6f
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
