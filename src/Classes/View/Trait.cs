using System.Collections.Generic;
using System.Linq;
using Ethereal.API;
using Ethereal.Attributes;
using UnityEngine;

namespace Ethereal.Classes.View;

/// <summary>
/// A helper class that allows easy viewing and editing of a Trait.
/// </summary>
/// <param name="gameObject"></param>
public sealed partial class TraitView(GameObject gameObject)
{
    /// <summary>
    /// A helper class that allows easy viewing and editing of a Trait.
    /// </summary>
    /// <param name="trait"></param>
    public TraitView(Trait trait)
        : this(trait.gameObject) { }

    public GameObject GameObject => gameObject;

    public readonly Trait Trait = gameObject.GetComponent<Trait>();

    [ForwardTo("Trait")]
    public partial int ID { get; set; }

    [ForwardTo("Trait")]
    public partial string Name { get; set; }

    [ForwardTo("Trait")]
    public partial string Description { get; set; }

    [ForwardTo("Trait")]
    public partial string Sidenote { get; set; }

    [ForwardTo("Trait")]
    public partial bool Aura { get; set; }

    [Forward("Trait.MaverickSkill")]
    public partial bool Maverick { get; set; }

    public List<PassiveEffect> PassiveEffects
    {
        get => [.. Trait.GetComponents<PassiveEffect>()];
        set
        {
            foreach (PassiveEffect passive in PassiveEffects)
                GameObject.DestroyImmediate(passive);

            foreach (PassiveEffect passive in value)
                GameObjects.CopyToGameObject(ref gameObject, passive);

            Trait.InitializeReferenceable();
        }
    }

    public List<EMonsterType> Types
    {
        get => [.. Trait.Types.Select(x => x.GetComponent<MonsterType>().Type)];
        set
        {
            Trait.Types =
            [
                .. value.Select(x =>
                    MonsterTypes.TryGet(x, out MonsterType type) ? type.gameObject : null
                ),
            ];
        }
    }

    [ForwardTo("Trait")]
    public partial Sprite Icon { get; set; }

    [ForwardTo("Trait")]
    public partial ESkillType SkillType { get; set; }
}
