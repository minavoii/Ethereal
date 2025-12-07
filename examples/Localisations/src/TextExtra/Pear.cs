using System.Collections.Generic;

namespace ExampleLocalisations.TextExtra;

internal static class Pear
{
    internal const int Id = 9902;

    internal const string Original = "Pear";

    internal static readonly LocalisationData.LocalisationDataEntry Entry = new()
    {
        ID = Id,
        StringContent = Original,
        StringContentEnglish = Original,
        StringContentFrench = "Poire",
    };

    internal static readonly Dictionary<string, string> Extras = new()
    {
        // Assuming a custom language named `Newlang` exists
        { "Newlang", "Paer" },
    };
}
