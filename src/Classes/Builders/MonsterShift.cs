using System.Collections.Generic;
using System.Linq;
using Ethereal.Classes.LazyValues;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a MonsterShift at runtime.
/// </summary>
/// <param name="Health"></param>
/// <param name="MainType"></param>
/// <param name="MonsterTypes"></param>
/// <param name="Elements"></param>
/// <param name="SignatureTrait"></param>
/// <param name="StartActions"></param>
/// <param name="ResetAction"></param>
/// <param name="Perks"></param>
public sealed record MonsterShiftBuilder(
    int? Health,
    EMonsterMainType? MainType,
    List<EMonsterType>? MonsterTypes,
    List<EElement>? Elements,
    LazyTrait? SignatureTrait,
    List<LazyAction>? StartActions,
    LazyAction? ResetAction,
    List<PerkInfosBuilder>? Perks
)
{
    public MonsterShiftBuilder()
        : this(null, null, null, null, null, null, null, null) { }

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
        };
}
