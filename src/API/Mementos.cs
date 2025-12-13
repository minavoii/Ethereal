using System;
using System.Threading.Tasks;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;

namespace Ethereal.API;

[BasicAPI]
public static partial class Mementos
{
    /// <summary>
    /// Get a memento by id.
    /// </summary>
    /// <param name="id"></param>
    public static async Task<MonsterMemento?> Get(int id) =>
        await Get(x => x?.BaseItem.ID == id, x => x?.ShiftedMemento.ID == id);

    /// <summary>
    /// Get a memento by name.
    /// </summary>
    /// <param name="name"></param>
    public static async Task<MonsterMemento?> Get(string name) =>
        await Get(x => x?.BaseItem.Name == name, x => x?.ShiftedMemento.Name == name);

    /// <summary>
    /// Get a memento using a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="predicateShifted"></param>
    /// <returns></returns>
    public static async Task<MonsterMemento?> Get(
        Predicate<ItemManager.MonsterMementoInstance?> predicate,
        Predicate<ItemManager.MonsterMementoInstance?> predicateShifted
    )
    {
        await WhenReady();
        return (
                GameController.Instance.ItemManager.MonsterMementos.Find(predicate)?.BaseItem
                ?? GameController
                    .Instance.ItemManager.MonsterMementos.Find(predicateShifted)
                    ?.ShiftedMemento
            ) as MonsterMemento;
    }

    /// <summary>
    /// Create a new memento and add it to the game's data.
    /// Optionally, adds the given meta upgrade to the witch's unlockable mementos inventory.
    /// </summary>
    /// <param name="memento"></param>
    /// <param name="metaUpgrade"></param>
    /// <param name="witchCategory"></param>
    public static async Task<MonsterMemento> Add(
        MonsterMementoBuilder memento,
        MetaUpgradeBuilder metaUpgrade,
        string? witchCategory = null
    ) => await Add(await metaUpgrade.Build(), await memento.Build(), null, witchCategory);

    /// <summary>
    /// Create a new memento and its shifted variant, and add them to the game's data. <br/>
    /// Optionally, adds the given meta upgrade to the witch's unlockable mementos inventory.
    /// </summary>
    /// <param name="memento"></param>
    /// <param name="shiftedMemento"></param>
    /// <param name="metaUpgrade"></param>
    /// <param name="WitchCategory"></param>
    public static async Task<MonsterMemento> Add(
        MonsterMementoBuilder memento,
        MonsterMementoBuilder shiftedMemento,
        MetaUpgradeBuilder metaUpgrade,
        string? witchCategory = null
    ) =>
        await Add(
            await metaUpgrade.Build(),
            await memento.Build(),
            await shiftedMemento.Build(),
            witchCategory
        );

    /// <summary>
    /// Create a new memento and its shifted variant (if any), then add them to the game's data. <br/>
    /// Optionally, adds the given meta upgrade to the witch's unlockable mementos inventory.
    /// </summary>
    /// <param name="memento"></param>
    /// <param name="shiftedMemento"></param>
    /// <param name="metaUpgrade"></param>
    /// <param name="witchCategory"></param>
    public static async Task<MonsterMemento> Add(
        MetaUpgrade metaUpgrade,
        MonsterMemento memento,
        MonsterMemento? shiftedMemento = null,
        string? witchCategory = null
    )
    {
        await WhenReady();

        MetaUpgrade goUpgrade = GameObjects.WithinGameObject(metaUpgrade);
        MonsterMemento goMemento = GameObjects.WithinGameObject(memento);

        if (shiftedMemento is not null)
        {
            MonsterMemento goShifted = GameObjects.WithinGameObject(shiftedMemento);

            GameController.Instance.ItemManager.MonsterMementos.Add(
                new() { BaseItem = goMemento, ShiftedMemento = goShifted }
            );

            await Referenceables.Add(goShifted);
        }
        else
        {
            GameController.Instance.ItemManager.MonsterMementos.Add(
                new() { BaseItem = goMemento, ShiftedMemento = goMemento }
            );
        }

        await Referenceables.Add(goMemento);
        await Referenceables.Add(goUpgrade);

        if (witchCategory is not null)
            _ = MetaUpgrades.AddToNPC(EMetaUpgradeNPC.Witch, witchCategory, goUpgrade);

        return goMemento;
    }
}
