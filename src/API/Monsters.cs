using System;
using System.Collections.Generic;
using System.Linq;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;
using UnityEngine;

namespace Ethereal.API;

[Deferrable]
public static partial class Monsters
{
    /// <summary>
    /// Get a monster by id.
    /// </summary>
    /// <param name="id"></param>
    [TryGet]
    private static Monster? Get(int id) =>
        Get(x =>
            x?.GetComponent<Monster>() is Monster monster
            && (monster.ID == id || monster.MonID == id)
        );

    /// <summary>
    /// Get a monster by name.
    /// </summary>
    /// <param name="name"></param>
    [TryGet]
    private static Monster? Get(string name) => Get(x => x?.GetComponent<Monster>()?.Name == name);

    private static Monster? Get(Predicate<GameObject?> predicate) =>
        GameController.Instance.CompleteMonsterList.Find(predicate)?.GetComponent<Monster>();

    /// <summary>
    /// Get all monsters.
    /// </summary>
    /// <returns></returns>
    [TryGet]
    private static List<Monster> GetAll() =>
        [
            .. GameController
                .Instance.CompleteMonsterList.Select(x => x?.GetComponent<Monster>()!)
                .Where(x => x is not null && x.Name != "Target Dummy")
                .Distinct(),
        ];

    /// <summary>
    /// Create a new monster and add it to the game's data.
    /// </summary>
    [Deferrable]
    private static void Add_Impl(MonsterBuilder monster) => Add_Impl(monster.Build());

    /// <summary>
    /// Create a new monster and add it to the game's data.
    /// </summary>
    [Deferrable]
    private static void Add_Impl(GameObject monster)
    {
        GameController.Instance.CompleteMonsterList.Add(monster);
        Referenceables.Add(monster.GetComponent<Monster>());
    }
}
