using System;
using System.Collections.Generic;
using System.Linq;
using Ethereal.Attributes;
using UnityEngine;

namespace Ethereal.API;

[Deferrable]
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
            .Where(x => x.value != null)
            .ToDictionary(x => x.id, x => x.value)!;

        API.SetReady();
    }

    /// <summary>
    /// Get a monster type.
    /// </summary>
    /// <param name="monsterType"></param>
    [TryGet]
    private static MonsterType? Get(EMonsterType monsterType) =>
        GameController.Instance.MonsterTypes.Find(x => x?.Type == monsterType);

    /// <summary>
    /// Get a monster type's GameObject.
    /// </summary>
    /// <param name="monsterType"></param>
    [TryGet]
    private static GameObject? GetObject(EMonsterType monsterType) =>
        GameController
            .Instance.CompleteMonsterList.SelectMany(x =>
                x?.GetComponent<SkillManager>()?.MonsterTypes
            )
            .FirstOrDefault(x => x?.GetComponent<MonsterType>()?.Type == monsterType);

    /// <summary>
    /// Set a monster type's icon.
    /// </summary>
    /// <param name="monsterType"></param>
    /// <param name="typeIcon"></param>
    [Deferrable]
    private static void UpdateIcon_Impl(EMonsterType monsterType, Sprite typeIcon)
    {
        if (TryGet(monsterType, out var type))
            type.TypeIcon = typeIcon;
    }
}
