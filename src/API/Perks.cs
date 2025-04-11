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
        foreach (GameObject monster in GameController.Instance.CompleteMonsterList)
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
        foreach (GameObject monster in GameController.Instance.CompleteMonsterList)
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
}
