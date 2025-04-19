using System.Collections.Generic;
using UnityEngine;

namespace Ethereal.API;

public static class Perks
{
    private static bool IsReady = false;

    /// <summary>
    /// Mark the API as ready.
    /// </summary>
    internal static void SetReady()
    {
        IsReady = true;
    }

    /// <summary>
    /// Get a perk by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>a perk if one was found; otherwise null.</returns>
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
    /// Get a perk by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="result"></param>
    /// <returns>true if the API is ready and a perk was found; otherwise, false.</returns>
    public static bool TryGet(int id, out PerkInfos result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(id);

        return result != null;
    }

    /// <summary>
    /// Get a perk by name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns>true if the API is ready and a perk was found; otherwise, false.</returns>
    public static bool TryGet(string name, out PerkInfos result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(name);

        return result != null;
    }

    /// <summary>
    /// Get all perks.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Get all actions.
    /// </summary>
    /// <param name="result"></param>
    /// <returns>true if the API is ready; otherwise, false.</returns>
    public static bool TryGetAll(out List<PerkInfos> result)
    {
        if (!IsReady)
            result = null;
        else
            result = GetAll();

        return result != null;
    }
}
