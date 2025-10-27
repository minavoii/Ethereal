using System;
using System.Collections.Generic;
using System.Linq;
using Ethereal.Attributes;

namespace Ethereal.API;

[BasicAPI]
public static partial class Perks
{
    /// <summary>
    /// Get a perk by id.
    /// </summary>
    /// <param name="id"></param>
    [TryGet]
    private static Perk? Get(int id) => Get(x => x?.ID == id);

    /// <summary>
    /// Get a perk by name.
    /// </summary>
    /// <param name="name"></param>
    [TryGet]
    private static Perk? Get(string name) => Get(x => x?.Name == name);

    /// <summary>
    /// Find a perk by monster.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static Perk? Get(Func<Perk?, bool> predicate) =>
        GameController
            .Instance.ActiveMonsterList.SelectMany(x =>
                x?.GetComponent<MonsterStats>()?.PerkInfosList
            )
            .Select(x => x?.Perk.GetComponent<Perk>())
            .FirstOrDefault(predicate);

    /// <summary>
    /// Get all perks.
    /// </summary>
    /// <returns>a list of all perks.</returns>
    [TryGet]
    private static List<Perk> GetAll() =>
        [
            .. GameController
                .Instance.ActiveMonsterList.SelectMany(x =>
                    x?.GetComponent<MonsterStats>()?.PerkInfosList
                )
                .Select(x => x?.Perk.GetComponent<Perk>()!)
                .Where(x => x is not null)
                .Distinct(),
        ];
}
