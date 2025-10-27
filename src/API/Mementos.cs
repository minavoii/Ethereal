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
    [TryGet]
    private static MonsterMemento? Get(int id) =>
        Get(x => x?.BaseItem.ID == id, x => x?.ShiftedMemento.ID == id);

    /// <summary>
    /// Get a memento by name.
    /// </summary>
    /// <param name="name"></param>
    [TryGet]
    private static MonsterMemento? Get(string name) =>
        Get(x => x?.BaseItem.Name == name, x => x?.ShiftedMemento.Name == name);

    private static MonsterMemento? Get(
        Predicate<ItemManager.MonsterMementoInstance?> predicate,
        Predicate<ItemManager.MonsterMementoInstance?> predicateShifted
    ) =>
        (
            GameController.Instance.ItemManager.MonsterMementos.Find(predicate)?.BaseItem
            ?? GameController
                .Instance.ItemManager.MonsterMementos.Find(predicateShifted)
                ?.ShiftedMemento
        ) as MonsterMemento;

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
    /// <param name="memento"></param>
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
    /// <param name="memento"></param>
    /// <param name="shiftedMemento"></param>
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
