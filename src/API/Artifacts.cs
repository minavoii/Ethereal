using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Ethereal.API;

public static class Artifacts
{
    public class ArtifactDescriptor
    {
        public int id;

        public string name = "";

        public string description = "";

        public ETargetType? targetType;

        public Sprite icon;

        public Sprite actionIconBig;

        public Sprite actionIconSmall;

        public List<ActionModifier> actions = [];

        public List<EElement> elements = [];
    }

    private static bool IsReady = false;

    private static readonly ConcurrentQueue<(
        ArtifactDescriptor descriptor,
        LocalisationData.LocalisationDataEntry localisationData,
        Dictionary<string, string> customLanguageEntries
    )> QueueAdd = new();

    private static readonly ConcurrentQueue<(int id, ArtifactDescriptor descriptor)> QueueUpdate =
        new();

    private static readonly ConcurrentQueue<(
        string name,
        ArtifactDescriptor descriptor
    )> QueueUpdateByName = new();

    internal static void ReadQueue()
    {
        IsReady = true;

        while (QueueAdd.TryDequeue(out var res))
        {
            if (res.customLanguageEntries == null)
            {
                if (res.localisationData == null)
                    Add(res.descriptor);
                else
                    Add(res.descriptor, res.localisationData);
            }
            else
                Add(res.descriptor, res.localisationData, res.customLanguageEntries);
        }

        while (QueueUpdate.TryDequeue(out var res))
            Update(res.id, res.descriptor);

        while (QueueUpdateByName.TryDequeue(out var res))
            Update(res.name, res.descriptor);
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

    public static void Add(ArtifactDescriptor descriptor)
    {
        LocalisationData.LocalisationDataEntry defaultLocalisation = new()
        {
            ID = descriptor.id,
            StringContent = descriptor.name,
            StringContentEnglish = descriptor.name,
        };

        // Defer loading until ready
        if (GameController.Instance?.ItemManager == null || !IsReady)
        {
            QueueAdd.Enqueue((descriptor, null, null));
            return;
        }

        Add(descriptor, defaultLocalisation);
    }

    public static void Add(
        ArtifactDescriptor descriptor,
        LocalisationData.LocalisationDataEntry localisationData
    )
    {
        // Defer loading until ready
        if (GameController.Instance?.ItemManager == null || !IsReady)
        {
            QueueAdd.Enqueue((descriptor, localisationData, null));
            return;
        }

        // Instantiating a BaseAction will call its `Update()` method,
        //   preventing us from running `AddComponent<BaseAction>()`,
        //   which instantiates the object by default
        // We clone an existing prefab and edit it to prevent this behavior
        Consumable original = (Consumable)
            GameController.Instance.ItemManager.Consumables.Find(x => x != null).BaseItem;

        Consumable artifact = Object.Instantiate(original);
        artifact.ID = descriptor.id;

        // We need the description to be empty if the user didn't provide any,
        //   so the default description can show up
        artifact.Description = descriptor.description;
        artifact.Action.GetComponent<BaseAction>().DescriptionOverride = descriptor.description;

        GameController.Instance.ItemManager.Consumables.Add(new() { BaseItem = artifact });
        WorldData.Instance.Referenceables.Add(artifact);

        Localisation.AddLocalisedText(localisationData);

        Update(descriptor.id, descriptor);

        Log.API.LogInfo($"Loaded artifact: {descriptor.name}");
    }

    public static void Add(
        ArtifactDescriptor descriptor,
        LocalisationData.LocalisationDataEntry localisationData,
        Dictionary<string, string> customLanguageEntries
    )
    {
        // Defer loading until ready
        if (GameController.Instance?.ItemManager == null || !IsReady)
        {
            QueueAdd.Enqueue((descriptor, localisationData, customLanguageEntries));
            return;
        }

        Add(descriptor);

        Localisation.AddLocalisedText(localisationData, customLanguageEntries);
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

        if (descriptor.elements.Count != 0)
            action.ElementsOverride = descriptor.elements;
    }
}
