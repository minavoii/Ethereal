using System.Collections.Generic;
using System.Linq;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a MonsterStats at runtime.
/// </summary>
/// <param name="BaseMaxHealth"></param>
/// <param name="PerkInfosList"></param>
public sealed record MonsterStatsBuilder(int BaseMaxHealth, List<PerkInfosBuilder> PerkInfosList)
{
    public MonsterStats Build()
    {
        return new()
        {
            BaseMaxHealth = BaseMaxHealth,
            PerkInfosList = [.. PerkInfosList.Select(x => x.Build())],
            Perks = [],
        };
    }
}
