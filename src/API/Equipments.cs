using System;
using System.Threading.Tasks;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;

namespace Ethereal.API;

[BasicAPI]
public static partial class Equipments
{
    /// <summary>
    /// Get an equipment by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="rarity"></param>
    public static async Task<Equipment?> Get(int id, ERarity rarity) =>
        await Get(
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
    public static async Task<Equipment?> Get(string name, ERarity rarity) =>
        await Get(
            x => x?.BaseItem.Name == name,
            x => x?.RareItem.Name == name,
            x => x?.EpicItem.Name == name,
            rarity
        );

    /// <summary>
    /// Get an equipment using a predicate.
    /// </summary>
    /// <param name="predicateBase"></param>
    /// <param name="predicateRare"></param>
    /// <param name="predicateEpic"></param>
    /// <param name="rarity"></param>
    /// <returns></returns>
    public static async Task<Equipment?> Get(
        Predicate<ItemManager.EquipmentItemInstance?> predicateBase,
        Predicate<ItemManager.EquipmentItemInstance?> predicateRare,
        Predicate<ItemManager.EquipmentItemInstance?> predicateEpic,
        ERarity rarity
    )
    {
        await API.WhenReady();

        return rarity switch
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
    }

    /// <summary>
    /// Create a new equipment and add it to the game's data.
    /// </summary>
    /// <param name="equipment"></param>
    public static async Task<Equipment> Add(EquipmentBuilder equipment) =>
        await Add(equipment.Build());

    /// <summary>
    /// Create a new equipment and add it to the game's data.
    /// </summary>
    /// <param name="equipment"></param>
    public static async Task<Equipment> Add(Equipment equipment)
    {
        await API.WhenReady();

        ItemManager.EquipmentItemInstance instance = new()
        {
            BaseItem = equipment,
            RareItem = equipment,
            EpicItem = equipment,
        };

        instance.Validate();

        GameController.Instance.ItemManager.Equipments.Add(instance);
        await Referenceables.Add(equipment);

        return equipment;
    }
}
