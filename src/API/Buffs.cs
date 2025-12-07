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

    private static Buff Get(Predicate<Buff?> predicate)
    {
        return Prefabs.Instance.AllBuffs.Find(predicate)
            ?? Prefabs.Instance.AllDebuffs.Find(predicate);
    }

    /// <summary>
    /// Set a buff's icon.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="icon"></param>
    [Deferrable]
    private static void UpdateIcon_Impl(int id, Sprite icon)
    {
        if (TryGet(id, out Buff buff))
            UpdateIcon(buff, icon);
    }

    /// <summary>
    /// Set a buff's icon.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    [Deferrable]
    private static void UpdateIcon_Impl(string name, Sprite icon)
    {
        if (TryGet(name, out Buff buff))
            UpdateIcon(buff, icon);
    }

    /// <summary>
    /// Set a buff's icon.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="icon"></param>
    private static void UpdateIcon(Buff buff, Sprite icon)
    {
        buff.MonsterHUDIconSmall = icon;
    }

    /// <summary>
    /// Create a new buff and add it to the game's data.
    /// </summary>
    /// <param name="buff"></param>
    [Deferrable]
    private static void Add_Impl(Buff buff)
    {
        GameObject buff_go = GameObjects.IntoGameObject(buff);

        foreach (PassiveEffect passive in buff.PassiveEffectList)
        {
            GameObjects.CopyToGameObject(ref buff_go, passive);
        }

        buff_go.GetComponent<Buff>().InitializeReferenceable();
        Referenceables.Add(buff_go.GetComponent<Buff>());

        if (buff.BuffType == EBuffType.Buff)
            Prefabs.Instance.AllBuffs.Add(buff_go.GetComponent<Buff>());
        else if (buff.BuffType == EBuffType.Debuff)
            Prefabs.Instance.AllDebuffs.Add(buff_go.GetComponent<Buff>());
    }
}
