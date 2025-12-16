using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;
using Ethereal.Classes.Views;

namespace Ethereal.API;

[BasicAPI]
public static partial class Equipments
{
    /// <summary>
    /// Get an equipment by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="rarity"></param>
    [GetObject, GetView(typeof(EquipmentView))]
    public static async Task<Equipment?> Get(int id, ERarity rarity) =>
        await Get(x => x?.ID == id, rarity);

    /// <summary>
    /// Get an equipment by name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="rarity"></param>
    [GetObject, GetView(typeof(EquipmentView))]
    public static async Task<Equipment?> Get(string name, ERarity rarity) =>
        await Get(x => x?.Name == name, rarity);

    /// <summary>
    /// Get an equipment using a predicate.
    /// </summary>
    /// <param name="predicateBase"></param>
    /// <param name="predicateRare"></param>
    /// <param name="predicateEpic"></param>
    /// <param name="rarity"></param>
    /// <returns></returns>
    [GetObject, GetView(typeof(EquipmentView))]
    public static async Task<Equipment?> Get(Predicate<Equipment> predicate, ERarity rarity) =>
        (await GetAll(rarity)).Find(predicate);

    /// <summary>
    /// Get all equipments of the given rarity.
    /// </summary>
    /// <param name="rarity"></param>
    /// <returns></returns>
    public static async Task<List<Equipment>> GetAll(ERarity rarity)
    {
        await WhenReady();
        return rarity switch
        {
            ERarity.Epic =>
            [
                .. GameController
                    .Instance.ItemManager.Equipments.Select(x => (x.EpicItem as Equipment)!)
                    .Where(x => x is not null),
            ],
            ERarity.Rare =>
            [
                .. GameController
                    .Instance.ItemManager.Equipments.Select(x => (x.RareItem as Equipment)!)
                    .Where(x => x is not null),
            ],
            ERarity.Common or _ =>
            [
                .. GameController
                    .Instance.ItemManager.Equipments.Select(x => (x.BaseItem as Equipment)!)
                    .Where(x => x is not null),
            ],
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
        await WhenReady();

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
