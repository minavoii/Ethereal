using System;
using System.Collections.Generic;
using System.Linq;
using Ethereal.Generator;
using UnityEngine;

namespace Ethereal.API;

[Deferreable]
public static partial class MonsterTypes
{
    public static Dictionary<EMonsterType, GameObject> NativeTypes = [];

    /// <summary>
    /// Mark the API as ready and run all deferred methods.
    /// </summary>
    internal static void SetReady()
    {
        NativeTypes = ((EMonsterType[])Enum.GetValues(typeof(EMonsterType)))
            .Select(x => (id: x, value: GetObject(x)))
            .ToDictionary(x => x.id, x => x.value);

        API.SetReady();
    }

    /// <summary>
    /// Get a monster type.
    /// </summary>
    /// <param name="monsterType"></param>
    /// <returns>a monster type if one was found; otherwise null.</returns>
    [TryGet]
    private static MonsterType Get(EMonsterType monsterType)
    {
        return GameController.Instance.MonsterTypes.Find(x => x?.Type == monsterType);
    }

    /// <summary>
    /// Get a monster type's GameObject.
    /// </summary>
    /// <param name="monsterType"></param>
    /// <returns>a monster type's GameObject if one was found, otherwise null.</returns>
    [TryGet]
    private static GameObject GetObject(EMonsterType monsterType)
    {
        foreach (var monster in GameController.Instance.ActiveMonsterList)
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
    /// Set a monster type's icon.
    /// </summary>
    /// <param name="monsterType"></param>
    /// <param name="typeIcon"></param>
    [Deferreable]
    private static void UpdateIcon_Impl(EMonsterType monsterType, Sprite typeIcon)
    {
        if (TryGet(monsterType, out var type))
            type.TypeIcon = typeIcon;
    }
}
