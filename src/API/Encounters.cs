using System.Collections.Generic;
using System.Linq;
using Ethereal.Attributes;
using Ethereal.Classes.LazyValues;

namespace Ethereal.API;

[Deferrable]
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
    [TryGet]
    private static MonsterEncounterSet? Get(EArea area, int zone) =>
        Biomes.TryGet(area, out TilemapLevelBiome biome)
            ? biome
                .MapBubbles.Select(x => x?.MonsterEncounterSet)
                .FirstOrDefault(x => x?.name == GetSetName(area, zone))
            : null;

    /// <summary>
    /// Get all monster encounter sets.
    /// </summary>
    /// <returns></returns>
    [TryGet]
    private static List<MonsterEncounterSet> GetAll() =>
        Biomes.TryGetAll(out List<TilemapLevelBiome> biomes)
            ?
            [
                .. biomes
                    .SelectMany(x => x?.MapBubbles)
                    .Select(x => x?.MonsterEncounterSet!)
                    .Where(x => x is not null)
                    .Distinct(),
            ]
            : [];

    /// <summary>
    /// Set wild monsters for the given encounter.
    /// </summary>
    /// <param name="encounter"></param>
    /// <param name="monsters"></param>
    /// <param name="encounterType"></param>
    /// <returns></returns>
    [Deferrable]
    private static void SetEnemies_Impl(
        MonsterEncounter encounter,
        List<LazyMonster> monsters,
        MonsterEncounter.EEncounterType? encounterType = null
    )
    {
        encounter.Enemies.Clear();
        encounter.Shifts.Clear();

        foreach (LazyMonster lazyMonster in monsters)
            if (lazyMonster.Get() is Monster monster)
                encounter.AddEnemy(monster.gameObject);

        if (encounterType.HasValue)
            encounter.EncounterType = encounterType.Value;
    }
}
