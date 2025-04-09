using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Ethereal.API;

public static class Artifacts
{
    /// <summary>
    /// A helper class that describes an artifact's properties.
    /// </summary>
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

    /// <summary>
    /// Mark the API as ready and run all deferred methods.
    /// </summary>
    internal static void SetReady()
    {
        IsReady = true;

        while (QueueAdd.TryDequeue(out var item))
        {
            if (item.customLanguageEntries == null)
            {
                if (item.localisationData == null)
                    Add(item.descriptor);
                else
                    Add(item.descriptor, item.localisationData);
            }
            else
                Add(item.descriptor, item.localisationData, item.customLanguageEntries);
        }

        while (QueueUpdate.TryDequeue(out var item))
            Update(item.id, item.descriptor);

        while (QueueUpdateByName.TryDequeue(out var item))
            Update(item.name, item.descriptor);
    }

    /// <summary>
    /// Get an artifact by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>an artifact if one was found; otherwise null.</returns>
    private static Consumable Get(int id)
    {
        return GameController
                .Instance.ItemManager.Consumables.Find(x => x?.BaseItem.ID == id)
                ?.BaseItem as Consumable;
    }

    /// <summary>
    /// Get an artifact by id.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>an artifact if one was found; otherwise null.</returns>
    private static Consumable Get(string name)
    {
        return GameController
                .Instance.ItemManager.Consumables.Find(x => x?.BaseItem.Name == name)
                ?.BaseItem as Consumable;
    }

    /// <summary>
    /// Get an artifact by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="result"></param>
    /// <returns>true if the API is ready and an artifact was found; otherwise, false.</returns>
    public static bool TryGet(int id, out Consumable result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(id);

        return result != null;
    }

    /// <summary>
    /// Get an artifact by name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns>true if the API is ready and an artifact was found; otherwise, false.</returns>
    public static bool TryGet(string name, out Consumable result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(name);

        return result != null;
    }

    /// <summary>
    /// Create a new artifact and add it to the game's data.
    /// </summary>
    /// <param name="descriptor"></param>
    public static void Add(ArtifactDescriptor descriptor)
    {
        LocalisationData.LocalisationDataEntry defaultLocalisation = new()
        {
            ID = descriptor.id,
            StringContent = descriptor.name,
            StringContentEnglish = descriptor.name,
        };

        // Defer loading until ready
        if (!IsReady)
        {
            QueueAdd.Enqueue((descriptor, null, null));
            return;
        }

        Add(descriptor, defaultLocalisation);
    }

    /// <summary>
    /// Create a new artifact and add it to the game's data alongside localisation data.
    /// </summary>
    /// <param name="descriptor"></param>
    /// <param name="localisationData"></param>
    public static void Add(
        ArtifactDescriptor descriptor,
        LocalisationData.LocalisationDataEntry localisationData
    )
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueAdd.Enqueue((descriptor, localisationData, null));
            return;
        }

        // Instantiating a BaseAction will call its `Update()` method,
        //   preventing us from running `AddComponent<BaseAction>()`,
        //   which instantiates the object by default
        // We clone an existing prefab and modify it to prevent this behavior
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

    /// <summary>
    /// Create a new artifact and add it to the game's data alongside localisation data,
    /// with translations for the provided custom languages.
    /// </summary>
    /// <param name="descriptor"></param>
    /// <param name="localisationData"></param>
    /// <param name="customLanguageEntries"></param>
    public static void Add(
        ArtifactDescriptor descriptor,
        LocalisationData.LocalisationDataEntry localisationData,
        Dictionary<string, string> customLanguageEntries
    )
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueAdd.Enqueue((descriptor, localisationData, customLanguageEntries));
            return;
        }

        Add(descriptor);

        Localisation.AddLocalisedText(localisationData, customLanguageEntries);
    }

    /// <summary>
    /// Overwrite an artifact's properties with values from a descriptor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="descriptor"></param>
    public static void Update(int id, ArtifactDescriptor descriptor)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdate.Enqueue((id, descriptor));
            return;
        }

        if (TryGet(id, out var artifact))
            Update(artifact, descriptor);
    }

    /// <summary>
    /// Overwrite an artifact's properties with values from a descriptor.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="descriptor"></param>
    public static void Update(string name, ArtifactDescriptor descriptor)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdateByName.Enqueue((name, descriptor));
            return;
        }

        if (TryGet(name, out var artifact))
            Update(artifact, descriptor);
    }

    /// <summary>
    /// Overwrite an artifact's properties with values from a descriptor.
    /// </summary>
    /// <param name="artifact"></param>
    /// <param name="descriptor"></param>
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
