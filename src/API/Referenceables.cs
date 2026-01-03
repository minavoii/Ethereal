using System;
using System.Collections.Generic;
using System.Linq;
using Ethereal.Attributes;
using Ethereal.CustomFlags;
using Ethereal.Patches;
using HarmonyLib;
using UnityEngine;

namespace Ethereal.API;

[Deferrable]
public static partial class Referenceables
{
    /// <summary>
    /// Add a referenceable to the game's data, and additionally to the cache if it's already built.
    /// </summary>
    /// <param name="referenceable"></param>
    [Deferrable]
    private static void Add_Impl(Referenceable referenceable)
    {
        var cache =
            (Dictionary<int, Referenceable>)
                AccessTools
                    .Field(typeof(WorldData), "referenceableCache")
                    .GetValue(WorldData.Instance);

        if (cache.Count != 0 && !cache.ContainsKey(referenceable.ID))
            cache.Add(referenceable.ID, referenceable);

        if (!WorldData.Instance.Referenceables.Any(x => x?.ID == referenceable.ID))
            WorldData.Instance.Referenceables.Add(referenceable);

        referenceable.transform.SetParent(WorldData.Instance.Storage());
    }

    /// <summary>
    /// Cleans up all added referenceables
    /// </summary>
    public static void Cleanup(string? scope = null)
    {
        var cache =
            (Dictionary<int, Referenceable>)
                AccessTools
                    .Field(typeof(WorldData), "referenceableCache")
                    .GetValue(WorldData.Instance);

        List<KeyValuePair<int, Referenceable>> custom = cache.Where(kvp => kvp.Value.gameObject.IsCustomObject(scope)).ToList();
        foreach (var kvp in custom)
        {
            cache.Remove(kvp.Key);
            WorldData.Instance.Referenceables.Remove(kvp.Value);
            GameObject.Destroy(kvp.Value.gameObject);
        }
    }

    /// <summary>
    /// Get a referenceable by id from the cache.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [TryGet]
    private static Referenceable GetCached(int id) => WorldData.Instance.GetReferenceable(id);

    /// <summary>
    /// Get a referenceable by id from the game's data.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [TryGet]
    private static Referenceable GetFromPrefab(int id) =>
        WorldData.Instance.Referenceables.FirstOrDefault(x => x?.ID == id);
}
