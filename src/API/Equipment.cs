using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Ethereal.API;

public static class Equipment
{
    public class EquipmentDescriptor(
        int id,
        string name,
        Sprite icon,
        int price,
        bool automaticPricing,
        global::Equipment.EEquipmentType type,
        ERarity rarity,
        bool aura,
        string description,
        List<PassiveEffect> passiveEffects
    )
    {
        public readonly int id = id;

        public readonly string name = name;

        public readonly Sprite icon = icon;

        public readonly int price = price;

        public readonly bool automaticPricing = automaticPricing;

        public readonly global::Equipment.EEquipmentType type = type;

        public readonly ERarity rarity = rarity;

        public readonly bool aura = aura;

        public readonly string description = description;

        public readonly List<PassiveEffect> passiveEffects = passiveEffects;
    }

    internal static bool IsReady = false;

    private static readonly ConcurrentQueue<EquipmentDescriptor> Queue = new();

    internal static void ReadQueue()
    {
        while (Queue.TryDequeue(out var res))
        {
            Add(res);
        }
    }

    public static void Add(EquipmentDescriptor descriptor)
    {
        // Defer loading until ready
        if (GameController.Instance?.ItemManager == null || !IsReady)
        {
            Queue.Enqueue(descriptor);
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
            EquipmentType = descriptor.type,
            EquipmentRarity = descriptor.rarity,
            AutomaticallySetPrice = descriptor.automaticPricing,
            Aura = descriptor.aura,
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

        API2.Log.LogInfo($"Loaded item: {descriptor.name}");
    }
}
