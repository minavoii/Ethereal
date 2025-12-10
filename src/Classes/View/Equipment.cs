using System.Collections.Generic;
using Ethereal.API;
using Ethereal.Attributes;
using UnityEngine;
using static Equipment;

namespace Ethereal.Classes.View;

/// <summary>
/// A helper class that allows easy viewing and editing of an Equipment.
/// </summary>
/// <param name="gameObject"></param>
public sealed partial class EquipmentView(GameObject gameObject)
{
    /// <summary>
    /// A helper class that allows easy viewing and editing of an Equipment.
    /// </summary>
    /// <param name="equipment"></param>
    public EquipmentView(Equipment equipment)
        : this(equipment.gameObject) { }

    public GameObject GameObject => gameObject;

    public readonly Equipment Equipment = gameObject.GetComponent<Equipment>();

    [ForwardTo("Equipment")]
    public partial int ID { get; set; }

    [ForwardTo("Equipment")]
    public partial string Name { get; set; }

    [Forward("Equipment.DescriptionOverride")]
    public partial string Description { get; set; }

    [Forward("Equipment.EquipmentType")]
    public partial EEquipmentType Type { get; set; }

    [Forward("Equipment.EquipmentRarity")]
    public partial ERarity Rarity { get; set; }

    [ForwardTo("Equipment")]
    public partial bool Aura { get; set; }

    [ForwardTo("Equipment")]
    public partial int Price { get; set; }

    [Forward("Equipment.AutomaticallySetPrice")]
    public partial bool AutomaticPricing { get; set; }

    [ForwardTo("Equipment")]
    public partial Sprite Icon { get; set; }

    public List<PassiveEffect> PassiveEffects
    {
        get => [.. Equipment.GetComponents<PassiveEffect>()];
        set
        {
            foreach (PassiveEffect passive in PassiveEffects)
                GameObject.DestroyImmediate(passive);

            foreach (PassiveEffect passive in value)
                GameObjects.CopyToGameObject(ref gameObject, passive);

            Equipment.InitializeReferenceable();
        }
    }
}
