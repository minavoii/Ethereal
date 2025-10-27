using System.Collections.Generic;
using System.Linq;
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
    GameObject ResetAction,
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
    public MonsterAI Build()
    {
        return new()
        {
            ResetAction = ResetAction,
            CannotUseDefaultAttack = CannotUseDefaultAttack,
            ExcludedFromTurnOrder = ExcludedFromTurnOrder,
            Traits = [.. Traits.Select(x => x.Build())],
            Scripting = [.. Scripting.Select(x => x.Build())],
            VoidPerks = [.. VoidPerks.Select(x => x.Build())],
            VoidPerksTier2 = [.. VoidPerksTier2.Select(x => x.Build())],
            VoidPerksTier3 = [.. VoidPerksTier3.Select(x => x.Build())],
            ChampionPerks = [.. ChampionPerks.Select(x => x.Build())],
        };
    }
}
