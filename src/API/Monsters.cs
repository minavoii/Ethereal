using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;
using Ethereal.Classes.Views;
using UnityEngine;

namespace Ethereal.API;

[BasicAPI]
public static partial class Monsters
{
    /// <summary>
    /// Get a monster by id.
    /// </summary>
    /// <param name="id"></param>
    [GetObject, GetView(typeof(MonsterView))]
    public static async Task<Monster?> Get(int id) =>
        await Get(x =>
            x?.GetComponent<Monster>() is Monster monster
            && (monster.ID == id || monster.MonID == id)
        );

    /// <summary>
    /// Get a monster by name.
    /// </summary>
    /// <param name="name"></param>
    [GetObject, GetView(typeof(MonsterView))]
    public static async Task<Monster?> Get(string name) =>
        await Get(x => x?.GetComponent<Monster>()?.Name == name);

    /// <summary>
    /// Get a monster using a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    [GetObject, GetView(typeof(MonsterView))]
    public static async Task<Monster?> Get(Predicate<GameObject?> predicate)
    {
        await WhenReady();
        return GameController.Instance.CompleteMonsterList.Find(predicate)?.GetComponent<Monster>();
    }

    /// <summary>
    /// Get all monsters.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<Monster>> GetAll()
    {
        await WhenReady();
        return
        [
            .. GameController
                .Instance.CompleteMonsterList.Select(x => x?.GetComponent<Monster>()!)
                .Where(x => x is not null && x.Name != "Target Dummy")
                .Distinct(),
        ];
    }

    /// <summary>
    /// Create a new monster and add it to the game's data.
    /// </summary>
    public static async Task<Monster> Add(MonsterBuilder monster) =>
        await Add(await monster.Build());

    /// <summary>
    /// Create a new monster and add it to the game's data.
    /// </summary>
    public static async Task<Monster> Add(GameObject monster)
    {
        await WhenReady();

        GameController.Instance.CompleteMonsterList.Add(monster);

        Monster goMonster = monster.GetComponent<Monster>();
        await Referenceables.Add(goMonster);

        return goMonster;
    }
}
