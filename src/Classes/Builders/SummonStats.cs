using System.Collections.Generic;
using System.Linq;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a MonsterStatsSummon at runtime.
/// </summary>
/// <param name="BaseMaxEssence"></param>
/// <param name="PerksToApplyToOwner"></param>
public sealed record SummonStatsBuilder(int BaseMaxEssence, List<PerkInfosBuilder> PerksToApplyToOwner)
{
    public MonsterStatsSummon Build()
    {
        return new()
        {
            BaseMaxEssence = BaseMaxEssence,
            PerksToApplyToOwner = [.. PerksToApplyToOwner.Select(x => x.Build())],
        };
    }
}
