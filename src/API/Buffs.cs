using System;
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
    public static async Task<Buff> Get(int id) => await Get(x => x?.ID == id);

    /// <summary>
    /// Get a buff by name.
    /// </summary>
    /// <param name="name"></param>
    public static async Task<Buff> Get(string name) => await Get(x => x?.Name == name);

    /// <summary>
    /// Get a buff using a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static async Task<Buff> Get(Predicate<Buff?> predicate)
    {
        await API.WhenReady();

        return Prefabs.Instance.AllBuffs.Find(predicate)
            ?? Prefabs.Instance.AllDebuffs.Find(predicate);
    }

    /// <summary>
    /// Create a new buff and add it to the game's data.
    /// </summary>
    /// <param name="buff"></param>
    public static async Task<Buff> Add(Buff buff)
    {
        await API.WhenReady();

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
