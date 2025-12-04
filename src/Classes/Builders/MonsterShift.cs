using System.Collections.Generic;
using System.Linq;
using Ethereal.Classes.LazyValues;
using UnityEngine;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a MonsterShift at runtime.
/// </summary>
/// <param name="Health">Override health of the shifted monster</param>
/// <param name="MainType">Archetype of the shifted monster</param>
/// <param name="MonsterTypes">Override Monster types of the shifted monster</param>
/// <param name="Elements">Override elements of the shifted monster</param>
/// <param name="SignatureTrait">Override Signature Trait of the shifted monster</param>
/// <param name="StartActions">Override Start Actions of the shifted monster</param>
/// <param name="ResetAction">Override Reset Action of the shifted monster</param>
/// <param name="Perks">Override Perks of the shifted monster</param>
/// <param name="BattleSpriteSource">Base Battle Sprite</param>
/// <param name="BattleSpriteReplace">Replacement Battle Sprite of the shifted monster</param>
/// <param name="OverworldSpriteSource">Base Overworld Sprite</param>
/// <param name="ProjectileSpriteSource">Base Projectile Sprite</param>
/// <param name="ProjectileSpriteReplace">Replacement Projectile Sprite</param>
/// <param name="PortraitSprite">Replacement Portrait Sprite</param>
/// <param name="ShiftVFX">Override Shift VFX of the shifted monster</param>
public sealed record MonsterShiftBuilder(
    int? Health,
    EMonsterMainType? MainType,
    List<EMonsterType>? MonsterTypes,
    List<EElement>? Elements,
    LazyTrait? SignatureTrait,
    List<LazyAction>? StartActions,
    LazyAction? ResetAction,
    List<PerkInfosBuilder>? Perks,
    Sprite? BattleSpriteSource,
    Sprite? BattleSpriteReplace,
    Sprite? OverworldSpriteSource,
    Sprite? OverworldSpriteReplace,
    Sprite? ProjectileSpriteSource,
    Sprite? ProjectileSpriteReplace,
    Sprite? PortraitSprite,
    GameObject? ShiftVFX
)
{
    public MonsterShiftBuilder()
        : this(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null) { }

    public MonsterShift Build() =>
        new()
        {
            MonsterTypesOverride =
                MonsterTypes != null
                    ?
                    [
                        .. MonsterTypes.Select(x =>
                            Ethereal.API.MonsterTypes.TryGetObject(x, out var type) ? type : null
                        ),
                    ]
                    : null,
            ElementsOverride = Elements,
            SignatureTraitOverride = SignatureTrait?.Get()?.gameObject,
            StartActionsOverride =
                StartActions != null ? [.. StartActions.Select(x => x.Get()?.gameObject)] : null,
            ResetPoiseActionOverride = ResetAction?.Get()?.gameObject,
            PerksOverride = [],
            ChangeMainType = MainType.HasValue,
            MainTypeOverride = MainType ?? EMonsterMainType.Hybrid,
            ChangeHealth = Health.HasValue,
            HealthOverride = Health ?? 0,
            BattleSpriteSource = BattleSpriteSource,
            BattleSpriteReplace = BattleSpriteReplace,
            OverworldSpriteSource = OverworldSpriteSource,
            OverworldSpriteReplace = OverworldSpriteReplace,
            ProjectileSpriteSource = ProjectileSpriteSource,
            ProjectileSpriteReplace = ProjectileSpriteReplace,
            PortraitSprite = PortraitSprite,
            ShiftVFX = ShiftVFX
        };
}
