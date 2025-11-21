using System;
using System.Collections.Generic;
using System.Linq;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;

namespace Ethereal.API;

[Deferrable]
public static partial class Keywords
{
    /// <summary>
    /// Get an keyword by name.
    /// </summary>
    /// <param name="name"></param>
    [TryGet]
    private static Keyword? Get(string name) => Get(x => x?.Name == name);

    private static Keyword? Get(Func<Keyword?, bool> predicate) =>
        KeywordManager
            .Instance.AllKeywords
            .Where(predicate)
            .FirstOrDefault()
        ?? WorldData.Instance.Referenceables.OfType<Keyword>().FirstOrDefault(predicate);

    /// <summary>
    /// Get all keywords.
    /// </summary>
    /// <returns></returns>
    [TryGet]
    private static List<Keyword> GetAll() => KeywordManager.Instance.AllKeywords;

    /// <summary>
    /// Create a new keyword and add it to the game's data.
    /// </summary>
    /// <param name="keyword"></param>
    /// <param name="localisationData"></param>
    /// <param name="customLanguageEntries"></param>
    [Deferrable]
    private static void Add_Impl(
        KeywordBuilder keyword,
        LocalisationData.LocalisationDataEntry localisationData,
        Dictionary<string, string> customLanguageEntries
    ) => Add_Impl(keyword.Build(), localisationData, customLanguageEntries);

    /// <summary>
    /// Create a new keyword and add it to the game's data.
    /// </summary>
    /// <param name="keyword"></param>
    /// <param name="localisationData"></param>
    /// <param name="customLanguageEntries"></param>
    [Deferrable]
    private static void Add_Impl(
        Keyword keyword,
        LocalisationData.LocalisationDataEntry localisationData,
        Dictionary<string, string> customLanguageEntries
    )
    {
        KeywordManager.Instance.AllKeywords.Add(keyword);
        Localisation.AddLocalisedText(localisationData, customLanguageEntries);
    }

    /// <summary>
    /// Overwrite an keyword's properties with values from a descriptor.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="descriptor"></param>
    [Deferrable]
    private static void Update_Impl(string name, Keyword descriptor)
    {
        if (TryGet(name, out var keyword))
            Update(keyword, descriptor);
    }

    /// <summary>
    /// Overwrite an keyword's properties with values from a descriptor.
    /// </summary>
    /// <param name="keyword"></param>
    /// <param name="descriptor"></param>
    private static void Update(Keyword keyword, Keyword descriptor)
    {
        if (descriptor.Name != string.Empty)
            keyword.Name = descriptor.Name;

        if (descriptor.Color != null)
            keyword.Color = descriptor.Color;

        if (descriptor.Identifier.Any())
            keyword.Identifier = descriptor.Identifier;

        if (descriptor.Description != null)
            keyword.Description = descriptor.Description;
    }
}
