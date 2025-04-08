using System.Collections.Concurrent;
using UnityEngine;

namespace Ethereal.API;

public static class Buffs
{
    private static readonly ConcurrentQueue<(int id, Sprite icon)> QueueUpdate = new();

    private static readonly ConcurrentQueue<(string name, Sprite icon)> QueueUpdateByName = new();

    private static bool IsReady = false;

    /// <summary>
    /// Mark the API as ready and run all deferred methods.
    /// </summary>
    internal static void ReadQueue()
    {
        IsReady = true;

        while (QueueUpdate.TryDequeue(out var item))
            UpdateIcon(item.id, item.icon);

        while (QueueUpdateByName.TryDequeue(out var item))
            UpdateIcon(item.name, item.icon);
    }

    /// <summary>
    /// Get a buff by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>a buff if one was found; otherwise null.</returns>
    public static Buff Get(int id)
    {
        return Prefabs.Instance.AllBuffs.Find(x => x.ID == id)
            ?? Prefabs.Instance.AllDebuffs.Find(x => x.ID == id);
    }

    /// <summary>
    /// Get a buff by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>a buff if one was found; otherwise null.</returns>
    public static Buff Get(string name)
    {
        return Prefabs.Instance.AllBuffs.Find(x => x.Name == name)
            ?? Prefabs.Instance.AllDebuffs.Find(x => x.Name == name);
    }

    /// <summary>
    /// Get a buff by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="result"></param>
    /// <returns>true if the API is ready and a buff was found; otherwise, false.</returns>
    public static bool TryGet(int id, out Buff result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(id);

        return result != null;
    }

    /// <summary>
    /// Get a buff by name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns>true if the API is ready and a buff was found; otherwise, false.</returns>
    public static bool TryGet(string name, out Buff result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(name);

        return result != null;
    }

    /// <summary>
    /// Set a buff's icon.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="icon"></param>
    public static void UpdateIcon(int id, Sprite icon)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdate.Enqueue((id, icon));
            return;
        }

        if (TryGet(id, out var buff))
            UpdateIcon(buff, icon);
    }

    /// <summary>
    /// Set a buff's icon.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    public static void UpdateIcon(string name, Sprite icon)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdateByName.Enqueue((name, icon));
            return;
        }

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
