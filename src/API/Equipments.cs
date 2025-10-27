using System.Collections.Generic;
using Ethereal.Generator;
using UnityEngine;

namespace Ethereal.API;

[Deferreable]
public static partial class Equipments
{
    /// <summary>
    /// A helper class that describes an equipment's properties.
    /// </summary>
    public class EquipmentDescriptor
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public Equipment.EEquipmentType? Type { get; set; }

        public ERarity? Rarity { get; set; }

        public bool? Aura { get; set; } = false;

        public int Price { get; set; } = 0;

        public bool? AutomaticPricing { get; set; } = true;

        public Sprite Icon { get; set; }

        public List<PassiveEffect> PassiveEffects { get; set; } = [];
    }

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
    /// <param name="descriptor"></param>
    [Deferreable]
    private static void Add_Impl(EquipmentDescriptor descriptor)
    {
        LocalisationData.LocalisationDataEntry defaultLocalisation = new()
        {
            ID = descriptor.Id,
            StringContent = descriptor.Name,
            StringContentEnglish = descriptor.Name,
        };

        Add_Impl(descriptor, defaultLocalisation);
    }

    /// <summary>
    /// Create a new artifact and add it to the game's data alongside localisation data,
    /// with translations for the provided custom languages.
    /// </summary>
    /// <param name="descriptor"></param>
    /// <param name="localisationData"></param>
    /// <param name="customLanguageEntries"></param>
    [Deferreable]
    private static void Add_Impl(
        EquipmentDescriptor descriptor,
        LocalisationData.LocalisationDataEntry localisationData,
        Dictionary<string, string> customLanguageEntries
    )
    {
        Add_Impl(descriptor);

        Localisation.AddLocalisedText(localisationData, customLanguageEntries);
    }

    /// <summary>
    /// Create a new equipment and add it to the game's data alongside localisation data.
    /// </summary>
    /// <param name="descriptor"></param>
    /// <param name="localisationData"></param>
    [Deferreable]
    private static void Add_Impl(
        EquipmentDescriptor descriptor,
        LocalisationData.LocalisationDataEntry localisationData
    )
    {
        string objectName = descriptor.Name.Replace(" ", "");

        GameObject go = new($"Equipment{objectName}_{descriptor.Rarity}");

        Equipment equipment = new()
        {
            ID = descriptor.Id,
            Name = descriptor.Name,
            Icon = descriptor.Icon,
            Price = descriptor.Price,
            EquipmentType = descriptor.Type ?? Equipment.EEquipmentType.Accessory,
            EquipmentRarity = descriptor.Rarity ?? ERarity.Common,
            AutomaticallySetPrice = descriptor.AutomaticPricing ?? false,
            Aura = descriptor.Aura ?? false,
            DescriptionOverride = descriptor.Description,
            PassiveEffectList = [],
        };

        Utils.GameObjects.CopyToGameObject(ref go, equipment);
        go.GetComponent<Equipment>().name = go.name;

        foreach (PassiveEffect passive in descriptor.PassiveEffects)
        {
            Utils.GameObjects.CopyToGameObject(ref go, passive);
        }

        // Copy PassiveEffect components into PassiveEffectList
        go.GetComponent<Equipment>().InitializeReferenceable();

        ItemManager.EquipmentItemInstance equItemInst = new()
        {
            BaseItem = go.GetComponent<Equipment>(),
            RareItem = go.GetComponent<Equipment>(),
            EpicItem = go.GetComponent<Equipment>(),
        };

        equItemInst.Validate();

        GameController.Instance.ItemManager.Equipments.Add(equItemInst);
        WorldData.Instance.Referenceables.Add(go.GetComponent<Equipment>());

        Localisation.AddLocalisedText(localisationData);

        Log.API.LogInfo($"Loaded equipment: {descriptor.Name}");
    }

    /// <summary>
    /// Overwrite an equipment's properties with values from a descriptor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="descriptor"></param>
    [Deferreable]
    private static void Update_Impl(int id, EquipmentDescriptor descriptor)
    {
        if (TryGet(id, ERarity.Common, out var common))
            Update(common, descriptor);
        else if (TryGet(id, ERarity.Rare, out var rare))
            Update(rare, descriptor);
        else if (TryGet(id, ERarity.Epic, out var epic))
            Update(epic, descriptor);
    }

    /// <summary>
    /// Overwrite an equipment's properties with values from a descriptor.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="rarity"></param>
    /// <param name="descriptor"></param>
    [Deferreable]
    private static void Update_Impl(string name, ERarity rarity, EquipmentDescriptor descriptor)
    {
        if (TryGet(name, rarity, out var equipment))
            Update(equipment, descriptor);
    }

    /// <summary>
    /// Overwrite an equipment's properties with values from a descriptor.
    /// </summary>
    /// <param name="equipment"></param>
    /// <param name="descriptor"></param>
    private static void Update(Equipment equipment, EquipmentDescriptor descriptor)
    {
        if (descriptor.Name != string.Empty)
            equipment.Name = descriptor.Name;

        if (descriptor.Icon != null)
            equipment.Icon = descriptor.Icon;

        if (descriptor.Price != 0)
            equipment.Price = descriptor.Price;

        if (descriptor.AutomaticPricing.HasValue)
            equipment.AutomaticallySetPrice = descriptor.AutomaticPricing.Value;

        if (descriptor.Type.HasValue)
            equipment.EquipmentType = descriptor.Type.Value;

        if (descriptor.Rarity.HasValue)
            equipment.EquipmentRarity = descriptor.Rarity.Value;

        if (descriptor.Aura.HasValue)
            equipment.Aura = descriptor.Aura.Value;

        if (descriptor.Description != string.Empty)
            equipment.DescriptionOverride = descriptor.Description;

        if (descriptor.PassiveEffects.Count != 0)
        {
            foreach (PassiveEffect comp in equipment.GetComponents<PassiveEffect>())
            {
                GameObject.DestroyImmediate(comp);
            }

            GameObject go = equipment.gameObject;

            foreach (PassiveEffect passive in descriptor.PassiveEffects)
            {
                Utils.GameObjects.CopyToGameObject(ref go, passive);
            }

            equipment.InitializeReferenceable();
        }
    }
}
