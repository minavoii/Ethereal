using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ethereal.API;

public static class MonsterTypes
{
    private static readonly ConcurrentQueue<(EMonsterType monsterType, Sprite typeIcon)> Queue =
        new();

    private static bool IsReady = false;

    public static Dictionary<EMonsterType, GameObject> NativeTypes = [];

    /// <summary>
    /// Mark the API as ready and run all deferred methods.
    /// </summary>
    internal static void SetReady()
    {
        NativeTypes = ((EMonsterType[])Enum.GetValues(typeof(EMonsterType)))
            .Select(x => (id: x, value: GetObject(x)))
            .ToDictionary(x => x.id, x => x.value);

        IsReady = true;

        while (Queue.TryDequeue(out var item))
            UpdateIcon(item.monsterType, item.typeIcon);
    }

    /// <summary>
    /// Get a monster type.
    /// </summary>
    /// <param name="monsterType"></param>
    /// <returns>a monster type if one was found; otherwise null.</returns>
    private static MonsterType Get(EMonsterType monsterType)
    {
        return GameController.Instance.MonsterTypes.Find(x => x?.Type == monsterType);
    }

    /// <summary>
    /// Get a monster type's GameObject.
    /// </summary>
    /// <param name="monsterType"></param>
    /// <returns>a monster type's GameObject if one was found, otherwise null.</returns>
    public static GameObject GetObject(EMonsterType monsterType)
    {
        foreach (var monster in GameController.Instance.CompleteMonsterList)
        {
            if (monster == null)
                continue;

            foreach (var type in monster.GetComponent<SkillManager>()?.MonsterTypes)
            {
                if (type?.GetComponent<MonsterType>()?.Type == monsterType)
                    return type;
            }
        }

        return null;
    }

    /// <summary>
    /// Get a monster type.
    /// </summary>
    /// <param name="monsterType"></param>
    /// <param name="result"></param>
    /// <returns>true if the API is ready and an artifact was found; otherwise, false.</returns>
    public static bool TryGet(EMonsterType monsterType, out MonsterType result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(monsterType);

        return result != null;
    }

    /// <summary>
    /// Set a monster type's icon.
    /// </summary>
    /// <param name="monsterType"></param>
    /// <param name="typeIcon"></param>
    public static void UpdateIcon(EMonsterType monsterType, Sprite typeIcon)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            Queue.Enqueue((monsterType, typeIcon));
            return;
        }

        if (TryGet(monsterType, out var type))
            type.TypeIcon = typeIcon;
    }
}
