using Ethereal.Generator;
using UnityEngine;

namespace Ethereal.API;

[Deferreable]
public static partial class Buffs
{
    /// <summary>
    /// Get a buff by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>a buff if one was found; otherwise null.</returns>
    [TryGet]
    private static Buff Get(int id)
    {
        return Prefabs.Instance.AllBuffs.Find(x => x.ID == id)
            ?? Prefabs.Instance.AllDebuffs.Find(x => x.ID == id);
    }

    /// <summary>
    /// Get a buff by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>a buff if one was found; otherwise null.</returns>
    [TryGet]
    private static Buff Get(string name)
    {
        return Prefabs.Instance.AllBuffs.Find(x => x.Name == name)
            ?? Prefabs.Instance.AllDebuffs.Find(x => x.Name == name);
    }

    /// <summary>
    /// Set a buff's icon.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="icon"></param>
    [Deferreable]
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
    [Deferreable]
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
}
