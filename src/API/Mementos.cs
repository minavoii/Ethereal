using System.Collections.Concurrent;
using UnityEngine;

namespace Ethereal.API;

public static class Mementos
{
    private static bool IsReady = false;

    private static readonly ConcurrentQueue<(int id, Sprite icon)> QueueUpdate = new();

    private static readonly ConcurrentQueue<(string name, Sprite icon)> QueueUpdateByName = new();

    internal static void ReadQueue()
    {
        IsReady = true;

        while (QueueUpdate.TryDequeue(out var res))
            UpdateIcon(res.id, res.icon);

        while (QueueUpdateByName.TryDequeue(out var res))
            UpdateIcon(res.name, res.icon);
    }

    public static MonsterMemento Get(int id)
    {
        ItemManager.MonsterMementoInstance item =
            GameController.Instance.ItemManager.MonsterMementos.Find(x => x?.BaseItem.ID == id);

        return item?.BaseItem as MonsterMemento;
    }

    public static MonsterMemento Get(string name)
    {
        ItemManager.MonsterMementoInstance item =
            GameController.Instance.ItemManager.MonsterMementos.Find(x => x?.BaseItem.Name == name);

        return item?.BaseItem as MonsterMemento;
    }

    public static void UpdateIcon(int id, Sprite icon)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdate.Enqueue((id, icon));
            return;
        }

        UpdateIcon(Get(id), icon);
    }

    public static void UpdateIcon(string name, Sprite icon)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdateByName.Enqueue((name, icon));
            return;
        }

        UpdateIcon(Get(name), icon);
    }

    private static void UpdateIcon(MonsterMemento memento, Sprite icon)
    {
        memento.Icon = icon;
    }
}
