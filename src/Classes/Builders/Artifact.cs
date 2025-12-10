using System.Collections.Generic;
using Ethereal.API;
using UnityEngine;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates an Artifact at runtime.
/// </summary>
/// <param name="ID"></param>
/// <param name="Name"></param>
/// <param name="TargetType"></param>
/// <param name="Icon"></param>
/// <param name="IconBig"></param>
/// <param name="IconSmall"></param>
/// <param name="Modifiers"></param>
/// <param name="DescriptionOverride"></param>
public sealed record ArtifactBuilder(
    int ID,
    string Name,
    ETargetType TargetType,
    Sprite Icon,
    Sprite IconBig,
    Sprite IconSmall,
    List<ActionModifier> Modifiers,
    string DescriptionOverride = ""
)
{
    public Consumable Build()
    {
        GameObject actionGo = GameObjects.IntoGameObject(
            new BaseAction()
            {
                Name = Name,
                DescriptionOverride = DescriptionOverride,
                SkillType = ESkillType.Shared,
                TargetType = TargetType,
                Types = [],
            }
        );

        BaseAction action = actionGo.GetComponent<BaseAction>();

        foreach (ActionModifier modifier in Modifiers)
            GameObjects.CopyToGameObject(ref actionGo, modifier);

        Consumable artifact = GameObjects.WithinGameObject(
            new Consumable()
            {
                ID = ID,
                Name = Name,
                Description = DescriptionOverride,
                Action = actionGo,
                Icon = Icon,
                ActionIconBig = IconBig,
                ActionIconSmall = IconSmall,
            }
        );

        GameObject parent = new();
        action.transform.parent = parent.transform;
        parent.SetActive(false);

        action.InitializeReferenceable();

        return artifact;
    }
}
