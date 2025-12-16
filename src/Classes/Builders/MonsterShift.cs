using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.API;
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
    public async Task<MonsterShift> Build() =>
        new()
        {
            MonsterTypesOverride = Types is not null
                ?
                [
                    .. await Task.WhenAll(
                        Types.Select(async x => (await MonsterTypes.Get(x))?.gameObject)
                    ),
                ]
                : null,
            ElementsOverride = Elements,
            SignatureTraitOverride = SignatureTrait is not null
                ? await SignatureTrait.GetObject()
                : null,
            StartActionsOverride = StartActions is not null
                ?
                [
                    .. await Task.WhenAll(
                        StartActions.Select(async x => (await x.Get())?.gameObject)
                    ),
                ]
                : null,
            ResetPoiseActionOverride = ResetAction is not null
                ? await ResetAction.GetObject()
                : null,
            PerksOverride = [],
            ChangeMainType = MainType.HasValue,
            MainTypeOverride = MainType ?? EMonsterMainType.Hybrid,
            ChangeHealth = Health.HasValue,
            HealthOverride = Health ?? 0,
        };
}
