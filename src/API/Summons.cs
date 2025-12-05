using System;
using System.Linq;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;
using UnityEngine;

namespace Ethereal.API;

[Deferrable]
public static partial class Summons
{
    /// <summary>
    /// Create a new summon and add it to the game's data.
    /// </summary>
    [Deferrable]
    private static void Add_Impl(SummonBuilder summonBuilder) => Add_Impl(summonBuilder.Build());

    /// <summary>
    /// Create a new summon and add it to the game's data.
    /// </summary>
    [Deferrable]
    private static void Add_Impl(GameObject summon)
    {
        Referenceables.Add(summon.GetComponent<Summon>());
    }

    /// <summary>
    /// Get a summon by id.
    /// </summary>
    /// <param name="id"></param>
    [TryGet]
    private static Summon? Get(int id) => Get(x => x?.ID == id);

    /// <summary>
    /// Get a summon by name.
    /// </summary>
    /// <param name="name"></param>
    [TryGet]
    private static Summon? Get(string name) => Get(x => x?.Name == name);

    /// <summary>
    /// Find a summon.
    /// </summary>
    /// <param name="predicate"></param>
    private static Summon? Get(Func<Summon?, bool> predicate)
    {
        return WorldData.Instance.Referenceables.OfType<Summon>().FirstOrDefault(predicate);
    }
}
