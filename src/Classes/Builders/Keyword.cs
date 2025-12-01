using System.Collections.Generic;
using UnityEngine;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a Keyword at runtime.
/// </summary>
/// <param name="Name"></param>
/// <param name="Identifier"></param>
/// <param name="Color"></param>
/// <param name="Description"></param>
public sealed record KeywordBuilder(
    string Name,
    List<string> Identifier,
    Color Color,
    string Description
)
{
    public Keyword Build()
    {
        var keyword = ScriptableObject.CreateInstance<Keyword>();
        keyword.Name = Name;
        keyword.Color = Color;
        keyword.Description = Description;
        keyword.Identifier = Identifier;
        return keyword;
    }
}
