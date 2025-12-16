using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Attributes;

namespace Ethereal.API;

[BasicAPI]
public static partial class Perks
{
    /// <summary>
    /// Get a perk by id.
    /// </summary>
    /// <param name="id"></param>
    [GetObject]
    public static async Task<Perk?> Get(int id) => await Get(x => x.ID == id);

    /// <summary>
    /// Get a perk by name.
    /// </summary>
    /// <param name="name"></param>
    [GetObject]
    public static async Task<Perk?> Get(string name) => await Get(x => x.Name == name);

    /// <summary>
    /// Find a perk using a predicate.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [GetObject]
    public static async Task<Perk?> Get(Predicate<Perk> predicate) =>
        (await GetAll()).Find(predicate);

    /// <summary>
    /// Get all perks.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<Perk>> GetAll()
    {
        await WhenReady();
        return
        [
            .. (await Monsters.GetAll())
                .SelectMany(x => x?.GetComponent<MonsterStats>()?.PerkInfosList)
                .Select(x => x?.Perk.GetComponent<Perk>()!)
                .Where(x => x is not null)
                .Distinct(),
        ];
    }
}
