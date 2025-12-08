using System.Collections.Generic;
using System.Linq;
using Ethereal.Attributes;
using UnityEngine;

namespace Ethereal.API;

[Deferrable]
public static partial class MonsterTypes
{
    /// <summary>
    /// Get a monster type.
    /// </summary>
    /// <param name="monsterType"></param>
    [TryGet]
    private static MonsterType? Get(EMonsterType monsterType) =>
        GameController.Instance.MonsterTypes.Find(x => x?.Type == monsterType);

    [TryGet]
    private static List<MonsterType> GetAll() =>
        [
            .. GameController.Instance.MonsterTypes.Where(x =>
                x.Type != EMonsterType.EnemySkills && x.Type != EMonsterType.UiSkills
            ),
        ];

    /// <summary>
    /// Set a monster type's icon.
    /// </summary>
    /// <param name="monsterType"></param>
    /// <param name="typeIcon"></param>
    [Deferrable]
    private static void UpdateIcon_Impl(EMonsterType monsterType, Sprite typeIcon)
    {
        if (TryGet(monsterType, out MonsterType type))
            type.TypeIcon = typeIcon;
    }
}
