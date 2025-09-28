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
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public ETargetType? TargetType { get; set; }

        public Sprite Icon { get; set; }

        public Sprite ActionIconBig { get; set; }

        public Sprite ActionIconSmall { get; set; }

        public List<ActionModifier> Actions { get; set; } = [];

        public List<EElement> Elements { get; set; } = [];
    }

    private static readonly QueueableAPI API = new();

    internal static void SetReady() => API.SetReady();

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
        if (!API.IsReady)
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
        if (!API.IsReady)
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
            ID = descriptor.Id,
            StringContent = descriptor.Name,
            StringContentEnglish = descriptor.Name,
        };

        // Defer loading until ready
        if (!API.IsReady)
        {
            API.Enqueue(() => Add(descriptor));
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
        if (!API.IsReady)
        {
            API.Enqueue(() => Add(descriptor, localisationData));
            return;
        }

        GameObject parent = new();
        BaseAction action = Utils.GameObjects.WithinGameObject(
            new BaseAction()
            {
                Name = descriptor.Name,
                DescriptionOverride = descriptor.Description,
                SkillType = ESkillType.Shared,
                Types = [],
            }
        );
        Consumable artifact = Utils.GameObjects.WithinGameObject(
            new Consumable()
            {
                ID = descriptor.Id,
                Name = descriptor.Name,
                Description = descriptor.Description,
            }
        );
        action.transform.parent = parent.transform;
        artifact.Action = action.gameObject;
        parent.SetActive(false);

        GameController.Instance.ItemManager.Consumables.Insert(0, new() { BaseItem = artifact });
        WorldData.Instance.Referenceables.Add(artifact);
        Localisation.AddLocalisedText(localisationData);

        Update(descriptor.Id, descriptor);

        Log.API.LogInfo($"Loaded artifact: {descriptor.Name}");
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
        if (!API.IsReady)
        {
            API.Enqueue(() => Add(descriptor, localisationData, customLanguageEntries));
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
        if (!API.IsReady)
        {
            API.Enqueue(() => Update(id, descriptor));
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
        if (!API.IsReady)
        {
            API.Enqueue(() => Update(name, descriptor));
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

        if (descriptor.Name != string.Empty)
        {
            artifact.Name = descriptor.Name;
            action.Name = descriptor.Name;
        }

        if (descriptor.Description != string.Empty)
        {
            artifact.Description = descriptor.Description;
            action.DescriptionOverride = descriptor.Description;
        }

        if (descriptor.TargetType.HasValue)
            action.TargetType = descriptor.TargetType.Value;

        if (descriptor.Icon != null)
            artifact.Icon = descriptor.Icon;

        if (descriptor.ActionIconBig != null)
            artifact.ActionIconBig = descriptor.ActionIconBig;

        if (descriptor.ActionIconSmall != null)
            artifact.ActionIconSmall = descriptor.ActionIconSmall;

        if (descriptor.Actions.Count != 0)
        {
            foreach (ActionModifier modifier in action.GetComponents<ActionModifier>())
                GameObject.DestroyImmediate(modifier);

            GameObject go = action.gameObject;

            foreach (ActionModifier modifier in descriptor.Actions)
                Utils.GameObjects.CopyToGameObject(ref go, modifier);

            action.InitializeReferenceable();
        }

        if (descriptor.Elements.Count != 0)
            action.ElementsOverride = descriptor.Elements;
    }
}
