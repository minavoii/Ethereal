
using UnityEngine;

namespace Ethereal.CustomFlags;

public sealed class CustomTagComponent : MonoBehaviour { }

public static class GameObjectExtensions
{
    public static bool IsCustomObject(this GameObject go)
    {
        return go?.GetComponent<CustomTagComponent>() != null;
    }
}