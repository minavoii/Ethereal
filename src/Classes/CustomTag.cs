
using UnityEngine;

namespace Ethereal.CustomFlags;

public sealed class CustomTagComponent : MonoBehaviour
{
    public string? Scope { get; set; }
}

public static class GameObjectExtensions
{
    public static void AddCustomTag(this GameObject go)
    {
        CustomTagComponent tag = go.AddComponent<CustomTagComponent>();
        tag.Scope = API.Scopes.CurrentScope;
    }

    public static bool IsCustomObject(this GameObject go, string? scope = null)
    {
        CustomTagComponent? tag = go?.GetComponent<CustomTagComponent>();
        if (tag == null)
        {
            return false;
        }
        if (string.IsNullOrEmpty(scope))
        {
            return true;
        }
        else
        {
            return tag.Scope == scope;
        }
    }
}