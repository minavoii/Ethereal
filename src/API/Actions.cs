using System;
using System.Collections.Generic;
using System.Linq;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;
using Ethereal.Classes.Wrappers;
using Ethereal.CustomFlags;
using HarmonyLib;
using UnityEngine;

namespace Ethereal.API;

[Deferrable]
public static partial class Actions
{
    /// <summary>
    /// A helper class that describes an action's properties.
    /// </summary>
    public class ActionDescriptor
    {
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public Aether? Cost { get; set; }

        public ETargetType? TargetType { get; set; }

        public EActionType? ActionType { get; set; }

        public bool? FreeAction { get; set; }

        public List<EElement> Elements { get; set; } = [];

        public List<EActionSubType> SubTypes { get; set; } = [];

        public Sprite? Icon { get; set; }

        public Sprite? IconSmall { get; set; }

        public Sprite? IconCutSmall { get; set; }

        public ESkillType? SkillType { get; set; }
    }

    /// <summary>
    /// Get an action by id.
    /// </summary>
    /// <param name="id"></param>
    [TryGet]
    private static BaseAction? Get(int id) => Get(x => x?.ID == id);

    /// <summary>
    /// Get an action by name.
    /// </summary>
    /// <param name="name"></param>
    [TryGet]
    private static BaseAction? Get(string name) => Get(x => x?.Name == name);

    private static BaseAction? Get(Func<BaseAction?, bool> predicate) =>
        GameController
            .Instance.MonsterTypes.SelectMany(x => x.Actions)
            .Where(predicate)
            .FirstOrDefault()
        ?? WorldData.Instance.Referenceables.OfType<BaseAction>().FirstOrDefault(predicate);

    /// <summary>
    /// Get all actions.
    /// </summary>
    /// <returns></returns>
    [TryGet]
    private static List<BaseAction> GetAll() =>
        [
            .. GameController
                .Instance.MonsterTypes.SelectMany(x => x.Actions)
                .Where(x => x.Name != "?????" && x.Name != "PoiseBreaker")
                .Distinct(),
        ];

    /// <summary>
    /// Create a new action and add it to the game's data.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="modifiers"></param>
    /// <param name="learnable"></param>
    [Deferrable]
    private static void Add_Impl(
        BaseActionBuilder action,
        List<ActionModifier> modifiers,
        bool learnable = false
    ) => Add_Impl(action.Build(), modifiers, action.VFXs, learnable);

    /// <summary>
    /// Create a new action and add it to the game's data.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="modifiers"></param>
    /// <param name="learnable"></param>
    [Deferrable]
    private static void Add_Impl(
        BaseAction action,
        List<ActionModifier> modifiers,
        List<VFX.ChildVFX> vfxChildren,
        bool learnable = false
    )
    {
        GameObject go = Utils.GameObjects.IntoGameObject(action);
        go.GetComponent<BaseAction>().enabled = false;
        go.AddCustomTag();

        if (action.IsFreeAction())
            go.GetComponent<BaseAction>().SetFreeAction(true);

        LateReferenceables.Queue(() =>
        {
            foreach (var modifier in modifiers)
            {
                if (modifier is ActionDamageWrapper damageWrapper)
                    damageWrapper.Unwrap();

                if (modifier is ActionSummonWrapper summonWrapper)
                    summonWrapper.Unwrap();

                Utils.GameObjects.CopyToGameObject(ref go, modifier);

                // Set the `Buffs` property here because it's private
                if (modifier is ActionApplyBuffWrapper applyBuff)
                {
                    AccessTools
                        .Field(typeof(ActionApplyBuff), "Buffs")
                        .SetValue(
                            go.GetComponent<ActionApplyBuff>(),
                            applyBuff.BuffDefines.Select(x => x.Build()).ToList()
                        );
                }
            }
        });

        if (vfxChildren.Count > 0)
        {
            VFX vfx = go.AddComponent<VFX>();
            vfx.enabled = false;
            vfx.Children = vfxChildren;
        }

        go.GetComponent<BaseAction>().InitializeReferenceable();
        Referenceables.Add(go.GetComponent<BaseAction>());

        if (learnable)
        {
            foreach (GameObject monsterType in action.Types)
                monsterType.GetComponent<MonsterType>().Actions.Add(go.GetComponent<BaseAction>());
        }
    }

    /// <summary>
    /// Cleans up all added custom actions
    /// </summary>
    public static void Cleanup(string? scope = null)
    {
        foreach (var monsterType in GameController.Instance.MonsterTypes)
        {
            List<BaseAction> customActions = monsterType.Actions.Where(a => a.gameObject.IsCustomObject(scope)).ToList();
            foreach (var action in customActions)
            {
                monsterType.Actions.Remove(action);
            }
        }
    }

    /// <summary>
    /// Overwrite an action's properties with values from a descriptor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="descriptor"></param>
    [Deferrable]
    private static void Update_Impl(int id, ActionDescriptor descriptor)
    {
        if (TryGet(id, out var action))
            Update(action, descriptor);
    }

    /// <summary>
    /// Overwrite an action's properties with values from a descriptor.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="descriptor"></param>
    [Deferrable]
    private static void Update_Impl(string name, ActionDescriptor descriptor)
    {
        if (TryGet(name, out var action))
            Update(action, descriptor);
    }

    /// <summary>
    /// Overwrite an action's properties with values from a descriptor.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="descriptor"></param>
    private static void Update(BaseAction action, ActionDescriptor descriptor)
    {
        if (descriptor.Name != string.Empty)
            action.Name = descriptor.Name;

        if (descriptor.Description != string.Empty)
            action.DescriptionOverride = descriptor.Description;

        if (descriptor.Cost != null)
            action.Cost = descriptor.Cost;

        if (descriptor.TargetType.HasValue)
            action.TargetType = descriptor.TargetType.Value;

        if (descriptor.ActionType.HasValue)
            action.ActionType = descriptor.ActionType.Value;

        if (descriptor.FreeAction.HasValue)
            action.SetFreeAction(descriptor.FreeAction.Value);

        if (descriptor.Elements.Count != 0)
            action.SetElements(descriptor.Elements);

        if (descriptor.SubTypes.Count != 0)
            AccessTools.Field(typeof(BaseAction), "subTypes").SetValue(action, descriptor.SubTypes);

        if (descriptor.Icon != null)
        {
            action.Icon = descriptor.Icon;
            action.ActionIconBig = descriptor.Icon;
        }

        if (descriptor.IconSmall != null)
            action.ActionIconSmall = descriptor.IconSmall;

        if (descriptor.IconCutSmall != null)
            action.ActionIconCutSmall = descriptor.IconCutSmall;

        if (descriptor.SkillType.HasValue)
            action.SkillType = descriptor.SkillType.Value;
    }
}
