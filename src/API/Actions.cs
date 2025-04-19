using System.Collections.Concurrent;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ethereal.API;

public static class Actions
{
    /// <summary>
    /// A helper class that describes an action's properties.
    /// </summary>
    public class ActionDescriptor
    {
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public Aether Cost { get; set; }

        public ETargetType? TargetType { get; set; }

        public EActionType? ActionType { get; set; }

        public bool? FreeAction { get; set; }

        public List<EElement> Elements { get; set; } = [];

        public List<EActionSubType> SubTypes { get; set; } = [];

        public Sprite Icon { get; set; }

        public Sprite IconSmall { get; set; }

        public Sprite IconCutSmall { get; set; }

        public ESkillType? SkillType { get; set; }
    }

    private static readonly ConcurrentQueue<(int id, ActionDescriptor descriptor)> QueueUpdate =
        new();

    private static readonly ConcurrentQueue<(
        string name,
        ActionDescriptor descriptor
    )> QueueUpdateByName = new();

    private static bool IsReady = false;

    /// <summary>
    /// Mark the API as ready and run all deferred methods.
    /// </summary>
    internal static void SetReady()
    {
        IsReady = true;

        while (QueueUpdate.TryDequeue(out var item))
            Update(item.id, item.descriptor);

        while (QueueUpdateByName.TryDequeue(out var item))
            Update(item.name, item.descriptor);
    }

    /// <summary>
    /// Get an action by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>an action if one was found; otherwise null.</returns>
    private static BaseAction Get(int id)
    {
        // Find action by monster type
        foreach (MonsterType type in GameController.Instance.MonsterTypes)
        {
            BaseAction action = type.Actions.Find(x => x.ID == id);

            if (action != null)
                return action;
        }

        return null;
    }

    /// <summary>
    /// Get an action by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>an action if one was found; otherwise null.</returns>
    private static BaseAction Get(string name)
    {
        // Find action by monster type
        foreach (MonsterType type in GameController.Instance.MonsterTypes)
        {
            BaseAction action = type.Actions.Find(x => x.Name == name);

            if (action != null)
                return action;
        }

        return null;
    }

    /// <summary>
    /// Get an action by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="result"></param>
    /// <returns>true if the API is ready and an action was found; otherwise, false.</returns>
    public static bool TryGet(int id, out BaseAction result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(id);

        return result != null;
    }

    /// <summary>
    /// Get an action by name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns>true if the API is ready and an action was found; otherwise, false.</returns>
    public static bool TryGet(string name, out BaseAction result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(name);

        return result != null;
    }

    /// <summary>
    /// Get all actions.
    /// </summary>
    /// <returns></returns>
    private static List<BaseAction> GetAll()
    {
        List<BaseAction> actions = [];

        foreach (MonsterType type in GameController.Instance.MonsterTypes)
        {
            foreach (BaseAction action in type.Actions)
            {
                if (
                    !actions.Contains(action)
                    && action.Name != "?????"
                    && action.Name != "PoiseBreaker"
                )
                    actions.Add(action);
            }
        }

        return actions;
    }

    /// <summary>
    /// Get all actions.
    /// </summary>
    /// <param name="result"></param>
    /// <returns>true if the API is ready; otherwise, false.</returns>
    public static bool TryGetAll(out List<BaseAction> result)
    {
        if (!IsReady)
            result = null;
        else
            result = GetAll();

        return result != null;
    }

    /// <summary>
    /// Overwrite an action's properties with values from a descriptor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="descriptor"></param>
    public static void Update(int id, ActionDescriptor descriptor)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdate.Enqueue((id, descriptor));
            return;
        }

        if (TryGet(id, out var action))
            Update(action, descriptor);
    }

    /// <summary>
    /// Overwrite an action's properties with values from a descriptor.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="descriptor"></param>
    public static void Update(string name, ActionDescriptor descriptor)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdateByName.Enqueue((name, descriptor));
            return;
        }

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
            action.FreeAction = descriptor.FreeAction.Value;

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
