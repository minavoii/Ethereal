using System.Collections.Generic;

namespace Randomizer.API;

/// <summary>
/// Represents randomizer data of a save file.
/// </summary>
/// <param name="views"></param>
/// <param name="mapping"></param>
internal class SaveData(Dictionary<int, SerializableView> views, Dictionary<int, int> mapping)
{
    public Dictionary<int, SerializableView> Views { get; set; } = views;

    public Dictionary<int, int> Mapping { get; set; } = mapping;
}
