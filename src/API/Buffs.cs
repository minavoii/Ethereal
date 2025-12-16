using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Attributes;
using UnityEngine;

namespace Ethereal.API;

[BasicAPI]
public static partial class Buffs
{
    /// <summary>
    /// Get a buff by id.
    /// </summary>
    /// <param name="id"></param>
    [GetObject]
    public static async Task<Buff?> Get(int id) => await Get(x => x.ID == id);

    /// <summary>
    /// Get a buff by name.
    /// </summary>
    /// <param name="name"></param>
    [GetObject]
    public static async Task<Buff?> Get(string name) => await Get(x => x.Name == name);

    /// <summary>
    /// Get a buff using a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    [GetObject]
    public static async Task<Buff?> Get(Predicate<Buff> predicate) =>
        (await GetAllBuffs()).Find(predicate) ?? (await GetAllDebuffs()).Find(predicate);

    public static async Task<List<Buff>> GetAllBuffs()
    {
        await WhenReady();
        return [.. Prefabs.Instance.AllBuffs.Where(x => x is not null)];
    }

    public static async Task<List<Buff>> GetAllDebuffs()
    {
        await WhenReady();
        return [.. Prefabs.Instance.AllDebuffs.Where(x => x is not null)];
    }

    /// <summary>
    /// Create a new buff and add it to the game's data.
    /// </summary>
    /// <param name="buff"></param>
    public static async Task<Buff> Add(Buff buff)
    {
        await WhenReady();

        GameObject go = GameObjects.IntoGameObject(buff);
        Buff goBuff = go.GetComponent<Buff>();

        foreach (PassiveEffect passive in buff.PassiveEffectList)
            GameObjects.CopyToGameObject(ref go, passive);

        goBuff.InitializeReferenceable();
        await Referenceables.Add(goBuff);

        if (buff.BuffType == EBuffType.Buff)
            Prefabs.Instance.AllBuffs.Add(goBuff);
        else if (buff.BuffType == EBuffType.Debuff)
            Prefabs.Instance.AllDebuffs.Add(goBuff);

        return goBuff;
    }
}
