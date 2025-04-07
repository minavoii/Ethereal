using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Ethereal.API;

public static class Equipments
{
    public class EquipmentDescriptor
    {
        public int id;

        public string name = "";

        public Sprite icon;

        public int price = 0;

        public bool? automaticPricing = true;

        public Equipment.EEquipmentType? type;

        public ERarity? rarity;

        public bool? aura = false;

        public string description = "";

        public List<PassiveEffect> passiveEffects = [];
    }

    private static bool IsReady = false;

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
        IsReady = true;

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

    public static Equipment Get(int id, ERarity rarity)
    {
        return rarity switch
        {
            ERarity.Epic => GameController
                .Instance.ItemManager.Equipments.Find(x => x.EpicItem?.ID == id)
                ?.BaseItem as Equipment,

            ERarity.Rare => GameController
                .Instance.ItemManager.Equipments.Find(x => x.RareItem?.ID == id)
                ?.RareItem as Equipment,

            ERarity.Common or _ => GameController
                .Instance.ItemManager.Equipments.Find(x => x.BaseItem?.ID == id)
                ?.BaseItem as Equipment,
        };
    }

    public static Equipment Get(string name, ERarity rarity)
    {
        return rarity switch
        {
            ERarity.Epic => GameController
                .Instance.ItemManager.Equipments.Find(x => x.EpicItem?.Name == name)
                ?.BaseItem as Equipment,

            ERarity.Rare => GameController
                .Instance.ItemManager.Equipments.Find(x => x.RareItem?.Name == name)
                ?.RareItem as Equipment,

            ERarity.Common or _ => GameController
                .Instance.ItemManager.Equipments.Find(x => x.BaseItem?.Name == name)
                ?.BaseItem as Equipment,
        };
    }

    public static bool TryGet(int id, ERarity rarity, out Equipment result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(id, rarity);

        return result != null;
    }

    public static bool TryGet(string name, ERarity rarity, out Equipment result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(name, rarity);

        return result != null;
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

        Equipment equipment = new()
        {
            ID = descriptor.id,
            Name = descriptor.name,
            Icon = descriptor.icon,
            Price = descriptor.price,
            EquipmentType = descriptor.type ?? Equipment.EEquipmentType.Accessory,
            EquipmentRarity = descriptor.rarity ?? ERarity.Common,
            AutomaticallySetPrice = descriptor.automaticPricing ?? false,
            Aura = descriptor.aura ?? false,
            DescriptionOverride = descriptor.description,
            PassiveEffectList = [],
        };

        equipment.Icon.name = objectName;
        equipment.Icon.texture.name = objectName;

        Utils.Converter.CopyToGameObject(ref go, equipment);
        go.GetComponent<Equipment>().name = go.name;

        foreach (PassiveEffect passive in descriptor.passiveEffects)
        {
            Utils.Converter.CopyToGameObject(ref go, passive);
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
        if (!IsReady)
        {
            QueueUpdate.Enqueue((id, descriptor));
            return;
        }

        if (TryGet(id, ERarity.Common, out var common))
            Update(common, descriptor);
        else if (TryGet(id, ERarity.Rare, out var rare))
            Update(rare, descriptor);
        else if (TryGet(id, ERarity.Epic, out var epic))
            Update(epic, descriptor);
    }

    public static void Update(string name, ERarity rarity, EquipmentDescriptor descriptor)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdateByName.Enqueue((name, rarity, descriptor));
            return;
        }

        if (TryGet(name, rarity, out var equipment))
            Update(equipment, descriptor);
    }

    private static void Update(Equipment equipment, EquipmentDescriptor descriptor)
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
