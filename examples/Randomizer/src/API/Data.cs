using System.Collections.Generic;
using System.Linq;
using Ethereal.API;
using UnityEngine;

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
    /// Get all available monster perks.
    /// </summary>
    /// <returns></returns>
    internal static List<PerkInfos> GetAllPerks()
    {
        List<PerkInfos> perks = [];

        foreach (GameObject monster in GameController.Instance.ActiveMonsterList)
        {
            if (monster == null)
                continue;

            foreach (PerkInfos info in monster.GetComponent<MonsterStats>().PerkInfosList)
            {
                int id = info.Perk.GetComponent<Perk>().ID;

                if (
                    perks.Find(x => x.Perk.GetComponent<Perk>().ID == id) == null
                    && info.Perk.GetComponent<Perk>().Name != "?????"
                )
                    perks.Add(info);
            }
        }

        return perks;
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
