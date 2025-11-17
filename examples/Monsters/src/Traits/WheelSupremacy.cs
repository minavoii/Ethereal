using System.Collections.Generic;
using System.IO;
using Ethereal.API;
using Ethereal.Classes.Builders;
using Ethereal.Classes.Wrappers;
using ExampleMonsters.CustomBuffs;

namespace ExampleMonsters.CustomTraits;

internal static class WheelSupremacy
{
    internal static readonly TraitBuilder Trait = new(
        ID: 1800,
        Name: "Wheel Supremacy",
        Description: "At the start of your turn: Applies {1} {Rotation} to self.\nFor every {Rotation} this monster has: Deals a {Wild Damage} {Hit} to all enemies.",
        Sidenote: "",
        SkillType: ESkillType.Shared,
        Types: [],
        PassiveEffects:
        [
            new PassiveGrantBuffWrapper(new(Rotation.Buff))
            {
                Trigger = PassiveTriggeredEffect.ETriggerType.TurnStart,
                Target = PassiveTriggeredEffect.ETriggerTarget.TriggeringMonster,
                ElementType = EElement.Water,
                TriggerEveryXCounter = 0,
                AuraType = EAuraType.Self,
                Conditions = [],
                CheckConditionsWhenQueued = false,
            },
            new PassiveGrantActionWrapper(new(WheelSupremacyAction.Action.ID), new(Rotation.Buff))
            {
                ActionType = EActionType.Any,
                ActionSubType = EActionSubType.Any,
                DedicationType = EDedicationType.Any,
                Trigger = PassiveTriggeredEffect.ETriggerType.OnReceiveBuff,
                Target = PassiveTriggeredEffect.ETriggerTarget.TriggeringMonster,
                ElementType = EElement.Any,
                AuraType = EAuraType.Self,
                TriggerEveryXCounter = 0,
                Conditions = [],
                CheckConditionsWhenQueued = false,
            },
        ],
        Icon: Sprites.LoadFromImage(
            Sprites.SpriteType.Trait,
            Path.Join(Plugin.CustomMonstersPath, "Trait_WheelSupremacy.png")
        )
    );
}

internal static class WheelSupremacyAction
{
    internal static readonly VFX.ChildVFX VFX = new()
    {
        VFX = VFXs.CreateCosmetic(Rotation.Animation),
        SpawnForEveryEnemy = true,
    };

    internal static readonly BaseActionBuilder Action = new(
        ID: 1901,
        Name: "Wheel Supremacy",
        Cost: new(),
        ActionType: EActionType.Spell,
        TargetType: ETargetType.AllEnemies,
        SkillType: ESkillType.Shared,
        Types: [],
        Elements: [EElement.Water],
        AnimationType: EActionAnimationType.CastSpell,
        ActionIconBig: new(),
        ActionIconSmall: new(),
        ActionIconCutSmall: new(),
        VFXs: [VFX]
    );

    internal static List<ActionModifier> Modifiers => [DamageWrapper];

    private static readonly ActionDamageWrapper DamageWrapper = new([new("Rotation")])
    {
        Damage = 2,
        HitCount = 1,
        AdditionalDamage = 1,
        AdditionalDamageType = ActionDamage.EAdditionalDamageType.PerBuff,
        BetweenHitsDelay = 0.2f,
    };
}
