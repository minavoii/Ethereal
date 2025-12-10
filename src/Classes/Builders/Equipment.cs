using System.Collections.Generic;
using Ethereal.API;
using UnityEngine;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates an Equipment at runtime.
/// </summary>
public sealed record EquipmentBuilder(
    int ID,
    string Name,
    Equipment.EEquipmentType Type,
    ERarity Rarity,
    Sprite Icon,
    List<PassiveEffect> PassiveEffects,
    string DescriptionOverride = "",
    bool Aura = false,
    int? Price = null
)
{
    public Equipment Build()
    {
        Equipment equipment = GameObjects.WithinGameObject(
            new Equipment()
            {
                ID = ID,
                Name = Name,
                EquipmentType = Type,
                EquipmentRarity = Rarity,
                Icon = Icon,
                Price = Price ?? 0,
                AutomaticallySetPrice = !Price.HasValue,
                Aura = Aura,
                DescriptionOverride = DescriptionOverride,
                PassiveEffectList = [],
            },
            $"Equipment{Name.Replace(" ", "")}_{Rarity}"
        );

        GameObject go = equipment.gameObject;
        equipment.name = go.name;

        foreach (PassiveEffect passive in PassiveEffects)
            GameObjects.CopyToGameObject(ref go, passive);

        // Copy PassiveEffect components into PassiveEffectList
        equipment.InitializeReferenceable();

        return equipment;
    }
}
