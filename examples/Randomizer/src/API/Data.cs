using System.Collections.Generic;
using System.Linq;
using Ethereal.API;

namespace Randomizer.API;

internal static class Data
{
    internal static List<PerkInfos> AllPerks = [];

    internal static List<Trait> SignatureTraits = [];

    /// <summary>
    /// Get all buffs related to the selected monster types.
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    internal static List<Buff> GetTypeBuffs(List<EMonsterType> types)
    {
        List<Buff> possibleBuffs = [];

        // Only include buffs derived from this monster's type
        foreach (EMonsterType type in types)
        {
            if (Buffs.TryGet(type.ToString(), out var buff))
                possibleBuffs.Add(buff);
        }

        return possibleBuffs;
    }

    /// <summary>
    /// Get signature traits of all monsters.
    /// </summary>
    /// <returns></returns>
    internal static List<Trait> GetAllSignatureTraits()
    {
        return
        [
            .. GameController
                .Instance.ActiveMonsterList.Select(x =>
                    x.GetComponent<SkillManager>()?.SignatureTrait?.GetComponent<Trait>()
                )
                .Where(x => x.Name != "?????"),
        ];
    }
}
