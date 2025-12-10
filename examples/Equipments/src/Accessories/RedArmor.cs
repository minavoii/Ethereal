using System.Collections.Generic;
using System.IO;
using Ethereal.API;
using Ethereal.Classes.Builders;
using static Equipment;

namespace ExampleEquipments.Accessory;

internal static class RedArmor
{
    private const int ID = 9801;

    private const string Name = "Red Armor";

    internal static readonly EquipmentBuilder Builder = new(
        ID: ID,
        Name: Name,
        DescriptionOverride: "At the start of your turn: {Shields} self for {0;2} x {0;1} and heals self for {1;2} x {1;1}.",
        Type: EEquipmentType.Accessory,
        Rarity: ERarity.Common,
        Price: 100,
        Icon: Sprites.LoadFromBundle(
            Path.Join(Plugin.EquipmentsPath, "RedArmor"),
            "Assets/Textures/Item/Item.prefab"
        ),
        PassiveEffects:
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
        ]
    );

    internal static readonly LocalisationData.LocalisationDataEntry LocalisationData = new()
    {
        ID = ID,
        StringContent = Name,
        StringContentEnglish = Name,
        StringContentFrench = "Armure Rouge",
    };

    internal static readonly Dictionary<string, string> CustomLanguageEntries = new()
    {
        // Assuming a custom language named `Newlang` exists
        { "Newlang", "Armor Red" },
    };
}
