using System.Collections.Generic;
using Ethereal.Generator;
using UnityEngine;

namespace Ethereal.API;

[BasicAPI]
public static partial class Perks
{
    /// <summary>
    /// Get a perk by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>a perk if one was found; otherwise null.</returns>
    [TryGet]
    private static PerkInfos Get(int id)
    {
        // Find perk by monster
        foreach (GameObject monster in GameController.Instance.ActiveMonsterList)
        {
            if (monster == null)
                continue;

            foreach (PerkInfos perk in monster.GetComponent<MonsterStats>().PerkInfosList)
            {
                if (perk.Perk.GetComponent<Perk>().ID == id)
                    return perk;
            }
        }

        return null;
    }

    /// <summary>
    /// Get a perk by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>a perk if one was found; otherwise null.</returns>
    [TryGet]
    private static PerkInfos Get(string name)
    {
        // Find perk by monster
        foreach (GameObject monster in GameController.Instance.ActiveMonsterList)
        {
            if (monster == null)
                continue;

            foreach (PerkInfos perk in monster.GetComponent<MonsterStats>().PerkInfosList)
            {
                if (perk.Perk.GetComponent<Perk>().Name == name)
                    return perk;
            }
        }

        return null;
    }

    /// <summary>
    /// Get all perks.
    /// </summary>
    /// <returns>a list of all perks.</returns>
    [TryGet]
    private static List<PerkInfos> GetAll()
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
}
