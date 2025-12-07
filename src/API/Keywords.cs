using System.Collections.Generic;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;

namespace Ethereal.API;

[Deferrable]
public static partial class Keywords
{
    /// <summary>
    /// Get a keyword by name.
    /// </summary>
    /// <param name="name"></param>
    [TryGet]
    private static Keyword? Get(string name) =>
        KeywordManager.Instance.AllKeywords.Find(x => x.Name == name);

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
    [Deferrable]
    private static void Add_Impl(KeywordBuilder keyword) => Add_Impl(keyword.Build());

    /// <summary>
    /// Create a new keyword and add it to the game's data.
    /// </summary>
    /// <param name="keyword"></param>
    [Deferrable]
    private static void Add_Impl(Keyword keyword) =>
        KeywordManager.Instance.AllKeywords.Add(keyword);
}
