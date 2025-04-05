using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Ethereal.API;

public static class Artifacts
{
    public class ArtifactDescriptor
    {
        public string name = "";

        public string description = "";

        public ETargetType? targetType;

        public Sprite icon;

        public Sprite actionIconBig;

        public Sprite actionIconSmall;

        public List<ActionModifier> actions = [];
    }

    private static bool IsReady = false;

    private static readonly ConcurrentQueue<(int id, ArtifactDescriptor descriptor)> QueueUpdate =
        new();

    private static readonly ConcurrentQueue<(
        string name,
        ArtifactDescriptor descriptor
    )> QueueUpdateByName = new();

    internal static void ReadQueue()
    {
        IsReady = true;

        while (QueueUpdate.TryDequeue(out var res))
            Update(res.id, res.descriptor);
    }

    public static Consumable Get(int id)
    {
        ItemManager.BaseItemInstance item = GameController.Instance.ItemManager.Consumables.Find(
            x => x?.BaseItem.ID == id
        );

        return item?.BaseItem as Consumable;
    }

    public static Consumable Get(string name)
    {
        ItemManager.BaseItemInstance item = GameController.Instance.ItemManager.Consumables.Find(
            x => x?.BaseItem.Name == name
        );

        return item?.BaseItem as Consumable;
    }

    public static void Update(int id, ArtifactDescriptor descriptor)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdate.Enqueue((id, descriptor));
            return;
        }

        Update(Get(id), descriptor);
    }

    public static void Update(string name, ArtifactDescriptor descriptor)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdateByName.Enqueue((name, descriptor));
            return;
        }

        Update(Get(name), descriptor);
    }

    private static void Update(Consumable artifact, ArtifactDescriptor descriptor)
    {
        BaseAction action = artifact.Action.GetComponent<BaseAction>();

        if (descriptor.name != string.Empty)
        {
            artifact.Name = descriptor.name;
            action.Name = descriptor.name;
        }

        if (descriptor.description != string.Empty)
        {
            artifact.Description = descriptor.description;
            action.DescriptionOverride = descriptor.description;
        }

        if (descriptor.targetType.HasValue)
            action.TargetType = descriptor.targetType.Value;

        if (descriptor.icon != null)
            artifact.Icon = descriptor.icon;

        if (descriptor.actionIconBig != null)
            artifact.ActionIconBig = descriptor.actionIconBig;

        if (descriptor.actionIconSmall != null)
            artifact.ActionIconSmall = descriptor.actionIconSmall;

        if (descriptor.actions.Count != 0)
        {
            foreach (ActionModifier modifier in action.GetComponents<ActionModifier>())
                Object.DestroyImmediate(modifier);

            GameObject go = action.gameObject;

            foreach (ActionModifier modifier in descriptor.actions)
                Utils.Converter.CopyToGameObject(ref go, modifier);

            action.InitializeReferenceable();
        }
    }
}
