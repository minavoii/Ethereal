using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ethereal.Patches;

internal class ReferenceableStorage : MonoBehaviour { }

public static class UIControllerExtensions
{
    private static readonly ConditionalWeakTable<UIController, GameObject> _extra
        = new();

    public static GameObject Extra(this UIController controller)
        => _extra.GetOrCreateValue(controller);

    public static Transform? Storage(this UIController controller)
    {
        ReferenceableStorage? storage = controller.GetComponentInChildren<ReferenceableStorage>();
        if (storage == null)
        {
            GameObject go = new("Storage");
            go.transform.parent = controller.transform;
            storage = go.AddComponent<ReferenceableStorage>();
        }

        return storage.transform;
    }
}