using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Ethereal.API;

public static class Equipment
{
    public class EquipmentDescriptor
    {
        public int id;

        public string name = "";

        public Sprite icon;

        public int price = 0;

        public bool? automaticPricing = true;

        public global::Equipment.EEquipmentType? type;

        public ERarity? rarity;

        public bool? aura = false;

        public string description = "";

        public List<PassiveEffect> passiveEffects = [];
    }

    internal static bool IsReady = false;

    private static readonly ConcurrentQueue<(
        EquipmentDescriptor descriptor,
        LocalisationData.LocalisationDataEntry localisationData,
        Dictionary<string, string> customLanguageEntries
    )> QueueAdd = new();

    private static readonly ConcurrentQueue<(int id, EquipmentDescriptor descriptor)> QueueUpdate =
        new();

    private static readonly ConcurrentQueue<(
        string name,
        ERarity rarity,
        EquipmentDescriptor descriptor
    )> QueueUpdateByName = new();

    internal static void ReadQueue()
    {
        while (QueueAdd.TryDequeue(out var res))
        {
            if (res.customLanguageEntries == null)
            {
                if (res.localisationData == null)
                    Add(res.descriptor);
                else
                    Add(res.descriptor, res.localisationData);
            }
            else
                Add(res.descriptor, res.localisationData, res.customLanguageEntries);
        }

        while (QueueUpdate.TryDequeue(out var res))
        {
            Update(res.id, res.descriptor);
        }

        while (QueueUpdateByName.TryDequeue(out var res))
        {
            Update(res.name, res.rarity, res.descriptor);
        }
    }

    public static void Add(EquipmentDescriptor descriptor)
    {
        LocalisationData.LocalisationDataEntry defaultLocalisation = new()
        {
            ID = descriptor.id,
            StringContent = descriptor.name,
            StringContentEnglish = descriptor.name,
        };

        // Defer loading until ready
        if (GameController.Instance?.ItemManager == null || !IsReady)
        {
            // Queue2.Enqueue((descriptor, defaultLocalisation));
            QueueAdd.Enqueue((descriptor, null, null));
            return;
        }

        Add(descriptor, defaultLocalisation);
    }

    public static void Add(
        EquipmentDescriptor descriptor,
        LocalisationData.LocalisationDataEntry localisationData
    )
    {
        // Defer loading until ready
        if (GameController.Instance?.ItemManager == null || !IsReady)
        {
            QueueAdd.Enqueue((descriptor, localisationData, null));
            return;
        }

        string objectName = descriptor.name.Replace(" ", "");

        GameObject go = new($"Equipment{objectName}_{descriptor.rarity}");

        global::Equipment equipment = new()
        {
            ID = descriptor.id,
            Name = descriptor.name,
            Icon = descriptor.icon,
            Price = descriptor.price,
            EquipmentType = descriptor.type ?? global::Equipment.EEquipmentType.Accessory,
            EquipmentRarity = descriptor.rarity ?? ERarity.Common,
            AutomaticallySetPrice = descriptor.automaticPricing ?? false,
            Aura = descriptor.aura ?? false,
            DescriptionOverride = descriptor.description,
            PassiveEffectList = [],
        };

        equipment.Icon.name = objectName;
        equipment.Icon.texture.name = objectName;

        Utils.Converter.CopyToGameObject(ref go, equipment);
        go.GetComponent<global::Equipment>().name = go.name;

        foreach (PassiveEffect passive in descriptor.passiveEffects)
        {
            Utils.Converter.CopyToGameObject(ref go, passive);
        }

        // Copy PassiveEffect components into PassiveEffectList
        go.GetComponent<global::Equipment>().InitializeReferenceable();

        ItemManager.EquipmentItemInstance equItemInst = new()
        {
            BaseItem = go.GetComponent<global::Equipment>(),
            RareItem = go.GetComponent<global::Equipment>(),
            EpicItem = go.GetComponent<global::Equipment>(),
        };

        equItemInst.Validate();

        GameController.Instance.ItemManager.Equipments.Add(equItemInst);
        WorldData.Instance.Referenceables.Add(go.GetComponent<global::Equipment>());

        Localisation.AddLocalisedText(localisationData);

        Log.API.LogInfo($"Loaded item: {descriptor.name}");
    }

    public static void Add(
        EquipmentDescriptor descriptor,
        LocalisationData.LocalisationDataEntry localisationData,
        Dictionary<string, string> customLanguageEntries
    )
    {
        // Defer loading until ready
        if (GameController.Instance?.ItemManager == null || !IsReady)
        {
            QueueAdd.Enqueue((descriptor, localisationData, customLanguageEntries));
            return;
        }

        Add(descriptor);

        Localisation.AddLocalisedText(localisationData, customLanguageEntries);
    }

    public static void Update(int id, EquipmentDescriptor descriptor)
    {
        // Defer loading until ready
        if (GameController.Instance?.ItemManager == null || !IsReady)
        {
            QueueUpdate.Enqueue((id, descriptor));
            return;
        }

        BaseItem common = GameController
            .Instance.ItemManager.Equipments.Find(x => x.BaseItem?.ID == id)
            ?.BaseItem;
        BaseItem rare = GameController
            .Instance.ItemManager.Equipments.Find(x => x.RareItem?.ID == id)
            ?.BaseItem;
        BaseItem epic = GameController
            .Instance.ItemManager.Equipments.Find(x => x.EpicItem?.ID == id)
            ?.BaseItem;

        BaseItem item = common ?? rare ?? epic;

        if (item != null)
            Update((global::Equipment)item, descriptor);
    }

    public static void Update(string name, ERarity rarity, EquipmentDescriptor descriptor)
    {
        // Defer loading until ready
        if (GameController.Instance?.ItemManager == null || !IsReady)
        {
            QueueUpdateByName.Enqueue((name, rarity, descriptor));
            return;
        }

        BaseItem item = rarity switch
        {
            ERarity.Epic => GameController
                .Instance.ItemManager.Equipments.Find(x => x.EpicItem?.Name == name)
                ?.BaseItem,

            ERarity.Rare => GameController
                .Instance.ItemManager.Equipments.Find(x => x.RareItem?.Name == name)
                ?.RareItem,

            ERarity.Common or _ => GameController
                .Instance.ItemManager.Equipments.Find(x => x.BaseItem?.Name == name)
                ?.BaseItem,
        };

        if (item != null)
            Update((global::Equipment)item, descriptor);
    }

    private static void Update(global::Equipment equipment, EquipmentDescriptor descriptor)
    {
        if (descriptor.name != string.Empty)
            equipment.Name = descriptor.name;

        if (descriptor.icon != null)
            equipment.Icon = descriptor.icon;

        if (descriptor.price != 0)
            equipment.Price = descriptor.price;

        if (descriptor.automaticPricing.HasValue)
            equipment.AutomaticallySetPrice = descriptor.automaticPricing.Value;

        if (descriptor.type.HasValue)
            equipment.EquipmentType = descriptor.type.Value;

        if (descriptor.rarity.HasValue)
            equipment.EquipmentRarity = descriptor.rarity.Value;

        if (descriptor.aura.HasValue)
            equipment.Aura = descriptor.aura.Value;

        if (descriptor.description != string.Empty)
            equipment.DescriptionOverride = descriptor.description;

        if (descriptor.passiveEffects.Count != 0)
        {
            foreach (PassiveEffect comp in equipment.GetComponents<PassiveEffect>())
            {
                Object.DestroyImmediate(comp);
            }

            GameObject go = equipment.gameObject;

            foreach (PassiveEffect passive in descriptor.passiveEffects)
            {
                Utils.Converter.CopyToGameObject(ref go, passive);
            }

            equipment.InitializeReferenceable();
        }
    }
}
