
using UnityEngine;

namespace Ethereal.CustomFlags;

public sealed class CustomTagComponent : MonoBehaviour
{
    public string? Scope { get; set; }
}

public static class GameObjectExtensions
{
    public static void AddCustomTag(this GameObject go, string? scope)
    {
        CustomTagComponent tag = go.AddComponent<CustomTagComponent>();
        tag.Scope = API.Scopes.CurrentScope;
    }

    public static bool IsCustomObject(this GameObject go)
    {
        return go?.GetComponent<CustomTagComponent>() != null;
    }
}