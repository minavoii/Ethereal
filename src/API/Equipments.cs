using System;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;

namespace Ethereal.API;

[Deferrable]
public static partial class Equipments
{
    /// <summary>
    /// Get an equipment by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="rarity"></param>
    [TryGet]
    private static Equipment? Get(int id, ERarity rarity) =>
        Get(
            x => x?.BaseItem.ID == id,
            x => x?.RareItem.ID == id,
            x => x?.EpicItem.ID == id,
            rarity
        );

    /// <summary>
    /// Get an equipment by name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="rarity"></param>
    [TryGet]
    private static Equipment? Get(string name, ERarity rarity) =>
        Get(
            x => x?.BaseItem.Name == name,
            x => x?.RareItem.Name == name,
            x => x?.EpicItem.Name == name,
            rarity
        );

    private static Equipment? Get(
        Predicate<ItemManager.EquipmentItemInstance?> predicateBase,
        Predicate<ItemManager.EquipmentItemInstance?> predicateRare,
        Predicate<ItemManager.EquipmentItemInstance?> predicateEpic,
        ERarity rarity
    ) =>
        rarity switch
        {
            ERarity.Epic => GameController
                .Instance.ItemManager.Equipments.Find(predicateEpic)
                ?.EpicItem as Equipment,

            ERarity.Rare => GameController
                .Instance.ItemManager.Equipments.Find(predicateRare)
                ?.RareItem as Equipment,

            ERarity.Common or _ => GameController
                .Instance.ItemManager.Equipments.Find(predicateBase)
                ?.BaseItem as Equipment,
        };

    /// <summary>
    /// Create a new equipment and add it to the game's data.
    /// </summary>
    /// <param name="equipment"></param>
    [Deferrable]
    private static void Add_Impl(EquipmentBuilder equipment) => Add_Impl(equipment.Build());

    /// <summary>
    /// Create a new equipment and add it to the game's data alongside localisation data.
    /// </summary>
    /// <param name="equipment"></param>
    [Deferrable]
    private static void Add_Impl(Equipment equipment)
    {
        ItemManager.EquipmentItemInstance instance = new()
        {
            BaseItem = equipment,
            RareItem = equipment,
            EpicItem = equipment,
        };

        instance.Validate();

        GameController.Instance.ItemManager.Equipments.Add(instance);
        Referenceables.Add(equipment);
    }
}
