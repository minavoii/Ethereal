using System.Collections.Concurrent;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ethereal.API;

public static class Actions
{
    public class ActionDescriptor
    {
        public string name = "";

        public string description = "";

        public Aether cost;

        public ETargetType? targetType;

        public EActionType? actionType;

        public bool? freeAction;

        public List<EElement> elements = [];

        public List<EActionSubType> subTypes = [];

        public Sprite icon;

        public Sprite iconSmall;

        public Sprite iconCutSmall;

        public ESkillType? skillType;
    }

    private static readonly ConcurrentQueue<(int id, ActionDescriptor descriptor)> QueueUpdate =
        new();

    private static readonly ConcurrentQueue<(
        string name,
        ActionDescriptor descriptor
    )> QueueUpdateByName = new();

    private static bool IsReady = false;

    internal static void ReadQueue()
    {
        IsReady = true;

        while (QueueUpdate.TryDequeue(out var item))
            Update(item.id, item.descriptor);

        while (QueueUpdateByName.TryDequeue(out var item))
            Update(item.name, item.descriptor);
    }

    public static BaseAction Get(int id)
    {
        // Find trait by monster type
        foreach (MonsterType type in GameController.Instance.MonsterTypes)
        {
            BaseAction action = type.Actions.Find(x => x.ID == id);

            if (action != null)
                return action;
        }

        return null;
    }

    public static BaseAction Get(string name)
    {
        // Find trait by monster type
        foreach (MonsterType type in GameController.Instance.MonsterTypes)
        {
            BaseAction action = type.Actions.Find(x => x.Name == name);

            if (action != null)
                return action;
        }

        return null;
    }

    public static bool TryGet(int id, out BaseAction result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(id);

        return result != null;
    }

    public static bool TryGet(string name, out BaseAction result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(name);

        return result != null;
    }

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

    private static void Update(BaseAction action, ActionDescriptor descriptor)
    {
        if (descriptor.name != string.Empty)
            action.Name = descriptor.name;

        if (descriptor.description != string.Empty)
            action.DescriptionOverride = descriptor.description;

        if (descriptor.cost != null)
            action.Cost = descriptor.cost;

        if (descriptor.targetType.HasValue)
            action.TargetType = descriptor.targetType.Value;

        if (descriptor.actionType.HasValue)
            action.ActionType = descriptor.actionType.Value;

        if (descriptor.freeAction.HasValue)
            action.FreeAction = descriptor.freeAction.Value;

        if (descriptor.elements.Count != 0)
            action.SetElements(descriptor.elements);

        if (descriptor.subTypes.Count != 0)
            AccessTools.Field(typeof(BaseAction), "subTypes").SetValue(action, descriptor.subTypes);

        if (descriptor.icon != null)
        {
            action.Icon = descriptor.icon;
            action.ActionIconBig = descriptor.icon;
        }

        if (descriptor.iconSmall != null)
            action.ActionIconSmall = descriptor.iconSmall;

        if (descriptor.iconCutSmall != null)
            action.ActionIconCutSmall = descriptor.iconCutSmall;

        if (descriptor.skillType.HasValue)
            action.SkillType = descriptor.skillType.Value;
    }
}
