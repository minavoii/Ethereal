using System;
using System.Collections.Generic;
using Ethereal.Attributes;
using UnityEngine;

namespace Ethereal.API;

[Deferrable]
public static partial class Artifacts
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

        public Sprite? Icon { get; set; }

        public Sprite? ActionIconBig { get; set; }

        public Sprite? ActionIconSmall { get; set; }

        public List<ActionModifier> Actions { get; set; } = [];

        public List<EElement> Elements { get; set; } = [];
    }

    /// <summary>
    /// Get an artifact by id.
    /// </summary>
    /// <param name="id"></param>
    [TryGet]
    private static Consumable? Get(int id) => Get(x => x?.BaseItem.ID == id);

    /// <summary>
    /// Get an artifact by id.
    /// </summary>
    /// <param name="name"></param>
    [TryGet]
    private static Consumable? Get(string name) => Get(x => x?.BaseItem.Name == name);

    private static Consumable? Get(Predicate<ItemManager.BaseItemInstance?> predicate)
    {
        return GameController.Instance.ItemManager.Consumables.Find(predicate)?.BaseItem
            as Consumable;
    }

    /// <summary>
    /// Create a new artifact and add it to the game's data.
    /// </summary>
    /// <param name="descriptor"></param>
    [Deferrable]
    private static void Add_Impl(ArtifactDescriptor descriptor)
    {
        LocalisationData.LocalisationDataEntry defaultLocalisation = new()
        {
            ID = descriptor.Id,
            StringContent = descriptor.Name,
            StringContentEnglish = descriptor.Name,
        };

        Add_Impl(descriptor, defaultLocalisation);
    }

    /// <summary>
    /// Create a new artifact and add it to the game's data alongside localisation data,
    /// with translations for the provided custom languages.
    /// </summary>
    /// <param name="descriptor"></param>
    /// <param name="localisationData"></param>
    /// <param name="customLanguageEntries"></param>
    [Deferrable]
    private static void Add_Impl(
        ArtifactDescriptor descriptor,
        LocalisationData.LocalisationDataEntry localisationData,
        Dictionary<string, string> customLanguageEntries
    )
    {
        Add_Impl(descriptor);

        Localisation.AddLocalisedText(localisationData, customLanguageEntries);
    }

    /// <summary>
    /// Create a new artifact and add it to the game's data alongside localisation data.
    /// </summary>
    /// <param name="descriptor"></param>
    /// <param name="localisationData"></param>
    [Deferrable]
    private static void Add_Impl(
        ArtifactDescriptor descriptor,
        LocalisationData.LocalisationDataEntry localisationData
    )
    {
        GameObject parent = new();
        BaseAction action = GameObjects.WithinGameObject(
            new BaseAction()
            {
                Name = descriptor.Name,
                DescriptionOverride = descriptor.Description,
                SkillType = ESkillType.Shared,
                Types = [],
            }
        );
        Consumable artifact = GameObjects.WithinGameObject(
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
        Referenceables.Add(artifact);
        Localisation.AddLocalisedText(localisationData);

        Update_Impl(descriptor.Id, descriptor);

        Log.API.LogInfo($"Loaded artifact: {descriptor.Name}");
    }

    /// <summary>
    /// Overwrite an artifact's properties with values from a descriptor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="descriptor"></param>
    [Deferrable]
    private static void Update_Impl(int id, ArtifactDescriptor descriptor)
    {
        if (TryGet(id, out var artifact))
            Update(artifact, descriptor);
    }

    /// <summary>
    /// Overwrite an artifact's properties with values from a descriptor.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="descriptor"></param>
    [Deferrable]
    private static void Update_Impl(string name, ArtifactDescriptor descriptor)
    {
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
                GameObjects.CopyToGameObject(ref go, modifier);

            action.InitializeReferenceable();
        }

        if (descriptor.Elements.Count != 0)
            action.ElementsOverride = descriptor.Elements;
    }
}
