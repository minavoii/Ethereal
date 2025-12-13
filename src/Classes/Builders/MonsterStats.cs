using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a MonsterStats at runtime.
/// </summary>
/// <param name="BaseMaxHealth"></param>
/// <param name="Perks"></param>
public sealed record MonsterStatsBuilder(int BaseMaxHealth, List<PerkInfosBuilder> Perks)
{
    public async Task<MonsterStats> Build() =>
        new()
        {
            BaseMaxHealth = BaseMaxHealth,
            PerkInfosList = [.. await Task.WhenAll(Perks.Select(x => x.Build()))],
            Perks = [],
        };
}
