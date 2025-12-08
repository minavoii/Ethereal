using System.Collections.Generic;
using System.Linq;
using Ethereal.Classes.LazyValues;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a MonsterShift at runtime.
/// </summary>
/// <param name="Health"></param>
/// <param name="MainType"></param>
/// <param name="Types"></param>
/// <param name="Elements"></param>
/// <param name="SignatureTrait"></param>
/// <param name="StartActions"></param>
/// <param name="ResetAction"></param>
/// <param name="Perks"></param>
public sealed record MonsterShiftBuilder(
    int? Health = null,
    EMonsterMainType? MainType = null,
    List<EMonsterType>? Types = null,
    List<EElement>? Elements = null,
    LazyTrait? SignatureTrait = null,
    List<LazyAction>? StartActions = null,
    LazyAction? ResetAction = null,
    List<PerkInfosBuilder>? Perks = null
)
{
    public MonsterShift Build() =>
        new()
        {
            MonsterTypesOverride = Types is not null
                ?
                [
                    .. Types.Select(x =>
                        Ethereal.API.MonsterTypes.TryGet(x, out MonsterType type)
                            ? type.gameObject
                            : null
                    ),
                ]
                : null,
            ElementsOverride = Elements,
            SignatureTraitOverride = SignatureTrait?.Get()?.gameObject,
            StartActionsOverride = StartActions is not null
                ? [.. StartActions.Select(x => x.Get()?.gameObject)]
                : null,
            ResetPoiseActionOverride = ResetAction?.Get()?.gameObject,
            PerksOverride = [],
            ChangeMainType = MainType.HasValue,
            MainTypeOverride = MainType ?? EMonsterMainType.Hybrid,
            ChangeHealth = Health.HasValue,
            HealthOverride = Health ?? 0,
        };
}
