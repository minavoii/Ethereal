using System.Collections.Generic;
using Ethereal.Attributes;
using HarmonyLib;
using UnityEngine;

namespace Ethereal.Classes.View;

/// <summary>
/// A helper class that allows easy viewing and editing of an Action.
/// </summary>
/// <param name="gameObject"></param>
public sealed partial class ActionView(GameObject gameObject)
{
    /// <summary>
    /// A helper class that allows easy viewing and editing of an Action.
    /// </summary>
    /// <param name="action"></param>
    public ActionView(BaseAction action)
        : this(action.gameObject) { }

    public GameObject GameObject => gameObject;

    public readonly BaseAction Action = gameObject.GetComponent<BaseAction>();

    [ForwardTo("Action")]
    public partial int ID { get; set; }

    [ForwardTo("Action")]
    public partial string Name { get; set; }

    [Forward("Action.DescriptionOverride")]
    public partial string Description { get; set; }

    [ForwardTo("Action")]
    public partial Aether Cost { get; set; }

    [ForwardTo("Action")]
    public partial ETargetType TargetType { get; set; }

    [ForwardTo("Action")]
    public partial EActionType ActionType { get; set; }

    [ForwardTo("Action")]
    public partial ESkillType SkillType { get; set; }

    public bool FreeAction
    {
        get => Action.IsFreeAction();
        set => Action.SetFreeAction(value);
    }

    public List<EElement> Elements
    {
        get => Action.Elements;
        set => Action.SetElements(value);
    }

    public List<EActionSubType> SubTypes
    {
        get => Action.GetSubTypes();
        set => AccessTools.Field(typeof(BaseAction), "subTypes").SetValue(Action, value);
    }

    public Sprite? IconBig
    {
        get => Action.Icon;
        set
        {
            Action.Icon = value;
            Action.ActionIconBig = value;
        }
    }

    [Forward("Action.ActionIconSmall")]
    public partial Sprite IconSmall { get; set; }

    [Forward("Action.ActionIconCutSmall")]
    public partial Sprite IconCutSmall { get; set; }
}
