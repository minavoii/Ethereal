using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a MonsterAI at runtime.
/// </summary>
/// <param name="ResetAction"></param>
/// <param name="Traits"></param>
/// <param name="Scripting"></param>
/// <param name="VoidPerks"></param>
/// <param name="VoidPerksTier2"></param>
/// <param name="VoidPerksTier3"></param>
/// <param name="ChampionPerks"></param>
/// <param name="CannotUseDefaultAttack"></param>
/// <param name="ExcludedFromTurnOrder"></param>
public sealed record MonsterAIBuilder(
    GameObject? ResetAction,
    List<MonsterAITraitBuilder> Traits,
    List<MonsterAIActionBuilder> Scripting,
    List<PerkInfosBuilder> VoidPerks,
    List<PerkInfosBuilder> VoidPerksTier2,
    List<PerkInfosBuilder> VoidPerksTier3,
    List<PerkInfosBuilder> ChampionPerks,
    bool CannotUseDefaultAttack,
    bool ExcludedFromTurnOrder
)
{
    public async Task<MonsterAI> Build() =>
        new()
        {
            ResetAction = ResetAction,
            CannotUseDefaultAttack = CannotUseDefaultAttack,
            ExcludedFromTurnOrder = ExcludedFromTurnOrder,
            Traits = [.. await Task.WhenAll(Traits.Select(x => x.Build()))],
            Scripting = [.. await Task.WhenAll(Scripting.Select(x => x.Build()))],
            VoidPerks = [.. await Task.WhenAll(VoidPerks.Select(x => x.Build()))],
            VoidPerksTier2 = [.. await Task.WhenAll(VoidPerksTier2.Select(x => x.Build()))],
            VoidPerksTier3 = [.. await Task.WhenAll(VoidPerksTier3.Select(x => x.Build()))],
            ChampionPerks = [.. await Task.WhenAll(ChampionPerks.Select(x => x.Build()))],
        };
}
