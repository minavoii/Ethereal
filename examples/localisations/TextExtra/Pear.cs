using System.Collections.Generic;

namespace Localisations.TextExtra;

internal static class Pear
{
    internal const int Id = 9902;

    internal const string Original = "Pear";

    internal static readonly LocalisationData.LocalisationDataEntry entry = new()
    {
        ID = Id,
        StringContent = Original,
        StringContentEnglish = Original,
        StringContentFrench = "Poire",
    };

    internal static readonly Dictionary<string, string> extras = new()
    {
        // Assuming a custom language named `Newlang` exists
        { "Newlang", "Raep" },
    };
}
