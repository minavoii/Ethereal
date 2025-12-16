using System.Collections.Generic;
using Ethereal.API;
using Ethereal.Attributes;
using UnityEngine;

namespace Ethereal.Classes.Views;

/// <summary>
/// A helper class that allows easy viewing and editing of an Artifact.
/// </summary>
/// <param name="gameObject"></param>
public sealed partial class ArtifactView(GameObject gameObject)
{
    /// <summary>
    /// A helper class that allows easy viewing and editing of an Artifact.
    /// </summary>
    /// <param name="artifact"></param>
    public ArtifactView(Consumable artifact)
        : this(artifact.gameObject) { }

    public GameObject GameObject => gameObject;

    public readonly Consumable Artifact = gameObject.GetComponent<Consumable>();

    public readonly BaseAction Action = gameObject
        .GetComponent<Consumable>()
        .Action.GetComponent<BaseAction>();

    [ForwardTo("Artifact")]
    public partial int ID { get; set; }

    public string Name
    {
        get => Artifact.Name;
        set
        {
            Artifact.Name = value;
            Action.Name = value;
        }
    }

    public string Description
    {
        get => Artifact.Description;
        set
        {
            Artifact.Description = value;
            Action.DescriptionOverride = value;
        }
    }

    [ForwardTo("Artifact")]
    public partial Sprite Icon { get; set; }

    [Forward("Artifact.ActionIconBig")]
    public partial Sprite IconBig { get; set; }

    [Forward("Artifact.ActionIconSmall")]
    public partial Sprite IconSmall { get; set; }

    [ForwardTo("Action")]
    public partial ETargetType TargetType { get; set; }

    public List<ActionModifier> Modifiers
    {
        get => [.. Action.GetComponents<ActionModifier>()];
        set
        {
            foreach (ActionModifier modifier in Modifiers)
                GameObject.DestroyImmediate(modifier);

            foreach (ActionModifier modifier in value)
                GameObjects.CopyToGameObject(ref gameObject, modifier);

            Action.InitializeReferenceable();
        }
    }
}
