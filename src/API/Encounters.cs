using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Attributes;
using Ethereal.Classes.LazyValues;
using UnityEngine;

namespace Ethereal.API;

[BasicAPI]
public static partial class Encounters
{
    /// <summary>
    /// Get the area's prefix used by monster sets.
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    private static string GetSetAreaPrefix(EArea area) =>
        area switch
        {
            EArea.PilgrimagePath => "PP1",
            EArea.ForbiddenFortress => "FF1",
            EArea.GardenDistrict => "GD1",
            EArea.Void => "Void",
            _ => "",
        };

    /// <summary>
    /// Get the area's suffix used by monster sets.
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    private static string GetSetAreaSuffix(EArea area) =>
        area switch
        {
            EArea.Void => "_Chernobog",
            _ => "",
        };

    /// <summary>
    /// Get the monster set's name from a given area and zone.
    /// </summary>
    /// <param name="area"></param>
    /// <param name="zone"></param>
    /// <returns></returns>
    private static string GetSetName(EArea area, int zone) =>
        $"MonsterEncounterSet_{GetSetAreaPrefix(area)}_MB_{zone}{GetSetAreaSuffix(area)}";

    /// <summary>
    /// Get a zone's monster encounter set.
    /// </summary>
    /// <param name="area"></param>
    /// <param name="zone">The 0-based zone number index</param>
    /// <returns></returns>
    public static async Task<MonsterEncounterSet?> Get(EArea area, int zone)
    {
        await API.WhenReady();

        return (await Biomes.Get(area))
            ?.MapBubbles.Select(x => x?.MonsterEncounterSet)
            .FirstOrDefault(x => x?.name == GetSetName(area, zone));
    }

    /// <summary>
    /// Get all monster encounter sets.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<MonsterEncounterSet>> GetAll()
    {
        await API.WhenReady(); //  Not neccessary?

        return
        [
            .. (await Biomes.GetAll())
                .SelectMany(x => x?.MapBubbles)
                .Select(x => x?.MonsterEncounterSet!)
                .Where(x => x is not null)
                .Distinct(),
        ];
    }

    /// <summary>
    /// Set wild monsters for the given encounter.
    /// </summary>
    /// <param name="encounter"></param>
    /// <param name="monsters"></param>
    /// <param name="encounterType"></param>
    /// <returns></returns>
    public static async Task SetEnemies(
        MonsterEncounter encounter,
        List<LazyMonster> monsters,
        MonsterEncounter.EEncounterType? encounterType = null
    )
    {
        await API.TaskSource.Task;

        encounter.Enemies.Clear();
        encounter.Shifts.Clear();

        foreach (LazyMonster lazyMonster in monsters)
            if (await lazyMonster.GetObject() is GameObject monster)
                encounter.AddEnemy(monster);

        if (encounterType.HasValue)
            encounter.EncounterType = encounterType.Value;
    }
}
