using System;
using Ethereal.Attributes;
using UnityEngine;

namespace Ethereal.API;

[Deferrable]
public static partial class Buffs
{
    /// <summary>
    /// Get a buff by id.
    /// </summary>
    /// <param name="id"></param>
    [TryGet]
    private static Buff Get(int id) => Get(x => x?.ID == id);

    /// <summary>
    /// Get a buff by name.
    /// </summary>
    /// <param name="name"></param>
    [TryGet]
    private static Buff Get(string name) => Get(x => x?.Name == name);

    private static Buff Get(Predicate<Buff?> predicate) =>
        Prefabs.Instance.AllBuffs.Find(predicate) ?? Prefabs.Instance.AllDebuffs.Find(predicate);

    /// <summary>
    /// Create a new buff and add it to the game's data.
    /// </summary>
    /// <param name="buff"></param>
    [Deferrable]
    private static void Add_Impl(Buff buff)
    {
        GameObject go = GameObjects.IntoGameObject(buff);
        Buff goBuff = go.GetComponent<Buff>();

        foreach (PassiveEffect passive in buff.PassiveEffectList)
        {
            GameObjects.CopyToGameObject(ref go, passive);
        }

        goBuff.InitializeReferenceable();
        Referenceables.Add(goBuff);

        if (buff.BuffType == EBuffType.Buff)
            Prefabs.Instance.AllBuffs.Add(goBuff);
        else if (buff.BuffType == EBuffType.Debuff)
            Prefabs.Instance.AllDebuffs.Add(goBuff);
    }
}
