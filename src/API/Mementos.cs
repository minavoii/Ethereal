using System;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;
using UnityEngine;

namespace Ethereal.API;

[Deferrable]
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
    [Deferrable]
    private static void UpdateIcon_Impl(int id, Sprite icon)
    {
        if (TryGet(id, out MonsterMemento memento))
            UpdateIcon(memento, icon);
    }

    /// <summary>
    /// Set a memento's icon.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    [Deferrable]
    private static void UpdateIcon_Impl(string name, Sprite icon)
    {
        if (TryGet(name, out MonsterMemento memento))
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
    /// Optionally, adds the given meta upgrade to the witch's unlockable mementos inventory.
    /// </summary>
    /// <param name="memento"></param>
    /// <param name="metaUpgrade"></param>
    /// <param name="witchCategory"></param>
    [Deferrable]
    private static void Add_Impl(
        MonsterMementoBuilder memento,
        MetaUpgradeBuilder metaUpgrade,
        string? witchCategory = null
    ) => Add(metaUpgrade.Build(), memento.Build(), null, witchCategory);

    /// <summary>
    /// Create a new memento and its shifted variant, and add them to the game's data. <br/>
    /// Optionally, adds the given meta upgrade to the witch's unlockable mementos inventory.
    /// </summary>
    /// <param name="memento"></param>
    /// <param name="shiftedMemento"></param>
    /// <param name="metaUpgrade"></param>
    /// <param name="WitchCategory"></param>
    [Deferrable]
    private static void Add_Impl(
        MonsterMementoBuilder memento,
        MonsterMementoBuilder shiftedMemento,
        MetaUpgradeBuilder metaUpgrade,
        string? witchCategory = null
    ) => Add(metaUpgrade.Build(), memento.Build(), shiftedMemento.Build(), witchCategory);

    /// <summary>
    /// Create a new memento and its shifted variant (if any), then add them to the game's data. <br/>
    /// Optionally, adds the given meta upgrade to the witch's unlockable mementos inventory.
    /// </summary>
    /// <param name="memento"></param>
    /// <param name="shiftedMemento"></param>
    /// <param name="metaUpgrade"></param>
    /// <param name="witchCategory"></param>
    private static void Add(
        MetaUpgrade metaUpgrade,
        MonsterMemento memento,
        MonsterMemento? shiftedMemento = null,
        string? witchCategory = null
    )
    {
        MetaUpgrade goUpgrade = GameObjects.WithinGameObject(metaUpgrade);
        MonsterMemento goMemento = GameObjects.WithinGameObject(memento);

        if (shiftedMemento is not null)
        {
            MonsterMemento goShifted = GameObjects.WithinGameObject(shiftedMemento);

            GameController.Instance.ItemManager.MonsterMementos.Add(
                new() { BaseItem = goMemento, ShiftedMemento = goShifted }
            );

            Referenceables.Add(goShifted);
        }
        else
        {
            GameController.Instance.ItemManager.MonsterMementos.Add(
                new() { BaseItem = goMemento, ShiftedMemento = goMemento }
            );
        }

        Referenceables.Add(goMemento);
        Referenceables.Add(goUpgrade);

        if (witchCategory != null)
            MetaUpgrades.AddToNPC(EMetaUpgradeNPC.Witch, witchCategory, goUpgrade);
    }
}
