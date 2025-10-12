using Ethereal.Generator;
using UnityEngine;

namespace Ethereal.API;

[Deferreable]
public static partial class Mementos
{
    /// <summary>
    /// Get a memento by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>a memento if one was found; otherwise null.</returns>
    [TryGet]
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
    /// <returns>a memento if one was found; otherwise null.</returns>
    [TryGet]
    private static MonsterMemento Get(string name)
    {
        return GameController
                .Instance.ItemManager.MonsterMementos.Find(x => x?.BaseItem.Name == name)
                ?.BaseItem as MonsterMemento;
    }

    /// <summary>
    /// Set a memento's icon.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="icon"></param>
    [Deferreable]
    private static void UpdateIcon_Impl(int id, Sprite icon)
    {
        if (TryGet(id, out var memento))
            UpdateIcon(memento, icon);
    }

    /// <summary>
    /// Set a memento's icon.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    [Deferreable]
    private static void UpdateIcon_Impl(string name, Sprite icon)
    {
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

    /// <summary>
    /// Create a new memento and add it to the game's data.
    /// </summary>
    /// <param name="descriptor"></param>
    [Deferreable]
    private static void Add_Impl(MonsterMemento memento)
    {
        GameObject go = new();
        Utils.GameObjects.CopyToGameObject(ref go, memento);

        GameController.Instance.ItemManager.MonsterMementos.Add(
            new() { BaseItem = go.GetComponent<MonsterMemento>() }
        );
    }

    /// <summary>
    /// Create a new memento and add it to the game's data.
    /// </summary>
    /// <param name="descriptor"></param>
    [Deferreable]
    private static void Add_Impl(MonsterMemento memento, MonsterMemento shiftedMemento)
    {
        GameObject go = new();
        GameObject go_shifted = new();

        Utils.GameObjects.CopyToGameObject(ref go, memento);
        Utils.GameObjects.CopyToGameObject(ref go_shifted, shiftedMemento);

        GameController.Instance.ItemManager.MonsterMementos.Add(
            new()
            {
                BaseItem = go.GetComponent<MonsterMemento>(),
                ShiftedMemento = go_shifted.GetComponent<MonsterMemento>(),
            }
        );
    }
}
