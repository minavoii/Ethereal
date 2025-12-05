using System;
using System.Collections.Generic;
using System.Linq;
using Ethereal.Attributes;
using Ethereal.CustomFlags;
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
        if (TryGet(id, out var buff))
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
        if (TryGet(name, out var buff))
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
        GameObject buff_go = Utils.GameObjects.IntoGameObject(buff);
        buff_go.AddCustomTag();

        foreach (PassiveEffect passive in buff.PassiveEffectList)
        {
            Utils.GameObjects.CopyToGameObject(ref buff_go, passive);
        }

        buff_go.GetComponent<Buff>().InitializeReferenceable();
        Referenceables.Add(buff_go.GetComponent<Buff>());

        if (buff.BuffType == EBuffType.Buff)
            Prefabs.Instance.AllBuffs.Add(buff_go.GetComponent<Buff>());
        else if (buff.BuffType == EBuffType.Debuff)
            Prefabs.Instance.AllDebuffs.Add(buff_go.GetComponent<Buff>());
    }

    /// <summary>
    /// Cleans up all added custom buffs
    /// </summary>
    public static void Cleanup(string? scope = null)
    {
        List<Buff> buffs = Prefabs.Instance.AllBuffs
            .Where(b => b.gameObject.IsCustomObject(scope))
            .ToList();
        List<Buff> debuffs = Prefabs.Instance.AllDebuffs
            .Where(b => b.gameObject.IsCustomObject(scope))
            .ToList();

        foreach (var buff in buffs)
        {
            Prefabs.Instance.AllBuffs.Remove(buff);
        }

        foreach (var debuff in debuffs)
        {
            Prefabs.Instance.AllDebuffs.Remove(debuff);
        }
    }
}
