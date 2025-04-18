using System.Collections.Generic;
using System.IO;
using Ethereal.API;

namespace ExampleEquipments.Accessory;

internal static class RedArmor
{
    internal const int Id = 9801;

    internal const string Name = "Red Armor";

    internal static readonly Equipments.EquipmentDescriptor descriptor = new()
    {
        Id = Id,
        Name = Name,
        Description =
            "At the start of your turn: {Shields} self for {0;2} x {0;1} and heals self for {1;2} x {1;1}.",
        Type = Equipment.EEquipmentType.Accessory,
        Rarity = ERarity.Common,
        Aura = false,
        Price = 100,
        AutomaticPricing = false,
        Icon = Sprites.LoadFromAsset(
            Path.Join(Plugin.EquipmentsPath, "RedArmor"),
            "Assets/Textures/Item/Item.prefab"
        ),
        PassiveEffects =
        [
            new PassiveGrantShield()
            {
                ShieldType = PassiveGrantShield.EShieldType.Normal,
                ShieldAmount = 10,
                ShieldCount = 3,
                Trigger = PassiveTriggeredEffect.ETriggerType.TurnStart,
                Target = PassiveTriggeredEffect.ETriggerTarget.TriggeringMonster,
                ElementType = EElement.Water,
                TriggerEveryXCounter = 0,
                AuraType = EAuraType.Self,
                Conditions = [],
                CheckConditionsWhenQueued = false,
            },
            new PassiveGrantHeal()
            {
                HealType = PassiveGrantHeal.EHealType.Normal,
                HealAmount = 50,
                HealCount = 2,
                Trigger = PassiveTriggeredEffect.ETriggerType.TurnStart,
                Target = PassiveTriggeredEffect.ETriggerTarget.TriggeringMonster,
                ElementType = EElement.Water,
                TriggerEveryXCounter = 0,
                AuraType = EAuraType.Self,
                Conditions = [],
                CheckConditionsWhenQueued = false,
            },
        ],
    };

    internal static readonly LocalisationData.LocalisationDataEntry localisationData = new()
    {
        ID = Id,
        StringContent = Name,
        StringContentEnglish = Name,
        StringContentFrench = "Armure Rouge",
    };

    internal static readonly Dictionary<string, string> customLanguageEntries = new()
    {
        // Assuming a custom language named `Newlang` exists
        { "Newlang", "Armor Red" },
    };
}
