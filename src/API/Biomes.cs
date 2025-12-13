using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Attributes;
using UnityEngine;

namespace Ethereal.API;

[BasicAPI]
public static partial class Biomes
{
    private static List<TilemapLevelBiome> LevelBiomes { get; set; } = [];

    /// <summary>
    /// Mark the API as ready and run all deferred methods.
    /// </summary>
    internal static void SetReady()
    {
        LevelBiomes =
        [
            .. Resources
                .FindObjectsOfTypeAll<TilemapLevelBiome>()
                .GroupBy(x => x.LevelID)
                .Select(x => x.First()),
        ];

        API.SetReady();
    }

    /// <summary>
    /// Get an area's name from an EArea.
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public static string GetAreaName(EArea area) =>
        area switch
        {
            EArea.PilgrimsRest => "Pilgrim's Rest",
            EArea.PilgrimagePath => "Pilgrimage Path",
            EArea.ForbiddenFortress => "Forbidden Fortress",
            EArea.GardenDistrict => "Garden District",
            EArea.Void => "Void",
            EArea.Tutorial => "Tutorial",
            _ => "",
        };

    /// <summary>
    /// Get a biome by area.
    /// </summary>
    /// <param name="area"></param>
    public static async Task<TilemapLevelBiome?> Get(EArea area)
    {
        await WhenReady();
        return LevelBiomes.Find(x => x?.Name == GetAreaName(area));
    }

    /// <summary>
    /// Get all biomes.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<TilemapLevelBiome>> GetAll()
    {
        await WhenReady();
        return LevelBiomes;
    }
}
