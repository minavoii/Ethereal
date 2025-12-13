using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Attributes;
using HarmonyLib;

namespace Ethereal.API;

[BasicAPI]
public static partial class Referenceables
{
    /// <summary>
    /// Add a referenceable to the game's data, and additionally to the cache if it's already built.
    /// </summary>
    /// <param name="referenceable"></param>
    public static async Task Add(Referenceable referenceable)
    {
        await API.WhenReady();

        Dictionary<int, Referenceable> cache =
            (Dictionary<int, Referenceable>)
                AccessTools
                    .Field(typeof(WorldData), "referenceableCache")
                    .GetValue(WorldData.Instance);

        if (cache.Count != 0 && !cache.ContainsKey(referenceable.ID))
            cache.Add(referenceable.ID, referenceable);

        if (!WorldData.Instance.Referenceables.Any(x => x?.ID == referenceable.ID))
            WorldData.Instance.Referenceables.Add(referenceable);
    }

    /// <summary>
    /// Get a referenceable from the cache.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static async Task<Referenceable> GetCached(int id)
    {
        await API.WhenReady();
        return WorldData.Instance.GetReferenceable(id);
    }

    /// <summary>
    /// Get a referenceable from the game's data.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static async Task<Referenceable> GetFromPrefab(int id)
    {
        await API.WhenReady();
        return WorldData.Instance.Referenceables.FirstOrDefault(x => x?.ID == id);
    }

    /// <summary>
    /// Get all referenceables of type T from the game's data.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<T>> GetManyOfType<T>()
        where T : Referenceable
    {
        await API.WhenReady();
        return WorldData.Instance.Referenceables.OfType<T>();
    }
}
