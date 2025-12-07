using System.Collections.Generic;
using UnityEngine;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a Keyword as a ScriptableObject.
/// </summary>
/// <param name="Name"></param>
/// <param name="Description"></param>
/// <param name="Identifier"></param>
/// <param name="Color"></param>
public sealed record KeywordBuilder(
    string Name,
    string Description,
    List<string> Identifier,
    Color Color
) : ScriptableObjectBuilder<Keyword>;
