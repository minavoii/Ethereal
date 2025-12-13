using System.Collections.Generic;
using System.Threading.Tasks;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;

namespace Ethereal.API;

[BasicAPI]
public static partial class Keywords
{
    /// <summary>
    /// Get a keyword by name.
    /// </summary>
    /// <param name="name"></param>
    public static async Task<Keyword?> Get(string name)
    {
        await API.WhenReady();
        return KeywordManager.Instance.AllKeywords.Find(x => x.Name == name);
    }

    /// <summary>
    /// Get all keywords.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<Keyword>> GetAll()
    {
        await API.WhenReady();
        return KeywordManager.Instance.AllKeywords;
    }

    /// <summary>
    /// Create a new keyword and add it to the game's data.
    /// </summary>
    /// <param name="keyword"></param>
    public static async Task<Keyword> Add(KeywordBuilder keyword) => await Add(keyword.Build());

    /// <summary>
    /// Create a new keyword and add it to the game's data.
    /// </summary>
    /// <param name="keyword"></param>
    public static async Task<Keyword> Add(Keyword keyword)
    {
        await API.WhenReady();

        KeywordManager.Instance.AllKeywords.Add(keyword);
        return keyword;
    }
}
