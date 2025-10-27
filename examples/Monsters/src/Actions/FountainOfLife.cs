using System.Collections.Generic;
using System.IO;
using Ethereal.API;
using Ethereal.Classes.Builders;
using Ethereal.Classes.Wrappers;

namespace ExampleMonsters.CustomActions;

internal static class FountainOfLife
{
    internal static readonly BaseActionBuilder Action = new(
        ID: 1903,
        Name: "Fountain of Life",
        Cost: new(EElement.Water, 3),
        ActionType: EActionType.Spell,
        TargetType: ETargetType.AllAllies,
        SkillType: ESkillType.Support,
        Types: [],
        Elements: [EElement.Water],
        AnimationType: EActionAnimationType.CastSpell,
        ActionIconBig: Sprites.LoadFromImage(
            Sprites.SpriteType.Action,
            Path.Join(Plugin.CustomMonstersPath, "ActionFountainOfLife_Large.png")
        ),
        ActionIconSmall: Sprites.LoadFromImage(
            Sprites.SpriteType.ActionSmall,
            Path.Join(Plugin.CustomMonstersPath, "ActionFountainOfLife_Small.png")
        ),
        ActionIconCutSmall: Sprites.LoadFromImage(
            Sprites.SpriteType.ActionCutSmall,
            Path.Join(Plugin.CustomMonstersPath, "ActionFountainOfLife_SmallCut.png")
        )
    );

    internal static List<ActionModifier> Modifiers => [Heal, RemoveBuff, ApplyBuff];

    private static readonly ActionHeal Heal = new()
    {
        HealCount = 3,
        HealAmount = 5,
        ApplyDelay = 0.6f,
        Target = ETargetType.AllAllies,
    };

    private static readonly ActionRemoveBuff RemoveBuff = new()
    {
        BuffAmount = 15,
        BuffType = EBuffType.Debuff,
        Target = ETargetType.AllAllies,
    };

    private static readonly ActionApplyBuffWrapper ApplyBuff = new([new("Regeneration", 3)])
    {
        ApplyType = ActionApplyBuff.EApplyType.AllListedBuffs,
        ApplyDelay = 1f,
        Target = ETargetType.AllAllies,
    };
}
