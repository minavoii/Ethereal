using System.Collections.Concurrent;
using UnityEngine;

namespace Ethereal.API;

public static class Buffs
{
    private static readonly ConcurrentQueue<(int id, Sprite icon)> QueueUpdate = new();

    private static readonly ConcurrentQueue<(string name, Sprite icon)> QueueUpdateByName = new();

    private static bool IsReady = false;

    internal static void ReadQueue()
    {
        IsReady = true;

        while (QueueUpdate.TryDequeue(out var res))
            UpdateIcon(res.id, res.icon);

        while (QueueUpdateByName.TryDequeue(out var res))
            UpdateIcon(res.name, res.icon);
    }

    public static Buff Get(int id)
    {
        return Prefabs.Instance.AllBuffs.Find(x => x.ID == id)
            ?? Prefabs.Instance.AllDebuffs.Find(x => x.ID == id);
    }

    public static Buff Get(string name)
    {
        return Prefabs.Instance.AllBuffs.Find(x => x.Name == name)
            ?? Prefabs.Instance.AllDebuffs.Find(x => x.Name == name);
    }

    public static void UpdateIcon(int id, Sprite icon)
    {
        // Defer loading until ready
        if (GameController.Instance?.CompleteMonsterList == null || !IsReady)
        {
            QueueUpdate.Enqueue((id, icon));
            return;
        }

        UpdateIcon(Get(id), icon);
    }

    public static void UpdateIcon(string name, Sprite icon)
    {
        // Defer loading until ready
        if (GameController.Instance?.CompleteMonsterList == null || !IsReady)
        {
            QueueUpdateByName.Enqueue((name, icon));
            return;
        }

        UpdateIcon(Get(name), icon);
    }

    private static void UpdateIcon(Buff buff, Sprite icon)
    {
        buff.MonsterHUDIconSmall = icon;
    }
}
