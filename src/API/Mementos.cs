using System.Collections.Concurrent;
using UnityEngine;

namespace Ethereal.API;

public static class Mementos
{
    private static bool IsReady = false;

    private static readonly ConcurrentQueue<(int id, Sprite icon)> QueueUpdate = new();

    private static readonly ConcurrentQueue<(string name, Sprite icon)> QueueUpdateByName = new();

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
    /// Get a memento by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>an action if one was found; otherwise null.</returns>
    private static MonsterMemento Get(int id)
    {
        return GameController
                .Instance.ItemManager.MonsterMementos.Find(x => x?.BaseItem.ID == id)
                ?.BaseItem as MonsterMemento;
    }

    /// <summary>
    /// Get a memento by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>an action if one was found; otherwise null.</returns>
    private static MonsterMemento Get(string name)
    {
        return GameController
                .Instance.ItemManager.MonsterMementos.Find(x => x?.BaseItem.Name == name)
                ?.BaseItem as MonsterMemento;
    }

    /// <summary>
    /// Get a memento by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="result"></param>
    /// <returns>true if the API is ready and an artifact was found; otherwise, false.</returns>
    public static bool TryGet(int id, out MonsterMemento result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(id);

        return result != null;
    }

    /// <summary>
    /// Get a memento by name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns>true if the API is ready and an artifact was found; otherwise, false.</returns>
    public static bool TryGet(string name, out MonsterMemento result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(name);

        return result != null;
    }

    /// <summary>
    /// Set a memento's icon.
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

        if (TryGet(id, out var memento))
            UpdateIcon(memento, icon);
    }

    /// <summary>
    /// Set a memento's icon.
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

        if (TryGet(name, out var memento))
            UpdateIcon(memento, icon);
    }

    /// <summary>
    /// Set a memento's icon.
    /// </summary>
    /// <param name="memento"></param>
    /// <param name="icon"></param>
    private static void UpdateIcon(MonsterMemento memento, Sprite icon)
    {
        memento.Icon = icon;
    }
}
