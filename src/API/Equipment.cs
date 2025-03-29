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
}
