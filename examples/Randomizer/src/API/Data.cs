using System.Collections.Generic;
using System.Linq;
using Ethereal.API;

namespace Randomizer.API;

internal static class Data
{
    internal static List<PerkInfos> AllPerks = [];

    internal static List<Trait> SignatureTraits = [];

    /// <summary>
    /// Get all buffs related to the selected monster types. <br/>
    /// Only include buffs derived from this monster's type.
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    internal static List<Buff> GetTypeBuffs(List<EMonsterType> types) =>
        [
            .. types
                .Select(x => Buffs.TryGet(x.ToString(), out Buff buff) ? buff : null)
                .Where(x => x is not null)
                .Distinct(),
        ];

    internal static List<PerkInfos> GetAllPerkInfos() =>
        [
            .. GameController
                .Instance.CompleteMonsterList.SelectMany(x =>
                    x?.GetComponent<MonsterStats>().PerkInfosList
                )
                .Where(x => x?.Perk.GetComponent<Perk>().Name != "?????")
                .Distinct(),
        ];
}
