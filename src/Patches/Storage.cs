using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ethereal.Patches;

internal class ReferenceableStorage : MonoBehaviour { }

public static class WorldDataExtensions
{
    private static readonly ConditionalWeakTable<WorldData, GameObject> _extra
        = new();

    public static GameObject Extra(this WorldData worldData)
        => _extra.GetOrCreateValue(worldData);

    public static Transform? Storage(this WorldData worldData)
    {
        ReferenceableStorage? storage = worldData.GetComponentInChildren<ReferenceableStorage>();
        if (storage == null)
        {
            GameObject go = new("Storage");
            go.transform.parent = worldData.transform;
            storage = go.AddComponent<ReferenceableStorage>();
        }

        return storage.transform;
    }
}