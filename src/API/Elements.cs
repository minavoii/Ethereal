using System.Collections.Concurrent;
using UnityEngine;

namespace Ethereal.API;

public static class Elements
{
    /// <summary>
    /// A helper class that describes an element's properties.
    /// </summary>
    public class ElementDescriptor()
    {
        public Sprite Icon { get; set; }

        public Sprite IconSmall { get; set; }

        public Sprite IconSmallEmpty { get; set; }

        public Sprite IconSmallFilled { get; set; }
    }

    private static readonly ConcurrentQueue<(
        EElement element,
        ElementDescriptor descriptor
    )> QueueUpdate = new();

    private static bool IsReady = false;

    /// <summary>
    /// Mark the API as ready and run all deferred methods.
    /// </summary>
    internal static void SetReady()
    {
        IsReady = true;

        while (QueueUpdate.TryDequeue(out var item))
            Update(item.element, item.descriptor);

        while (QueueUpdate.TryDequeue(out var item))
            Update(item.element, item.descriptor);
    }

    /// <summary>
    /// Set an element's icon.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="descriptor"></param>
    public static void Update(EElement element, ElementDescriptor descriptor)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdate.Enqueue((element, descriptor));
            return;
        }

        if (descriptor.Icon != null)
            Prefabs.Instance.ElementIcons[(int)element] = descriptor.Icon;

        if (descriptor.IconSmall != null)
            Prefabs.Instance.ElementIconsSmall[(int)element] = descriptor.IconSmall;

        if (descriptor.IconSmallEmpty != null)
            Prefabs.Instance.ElementIconsSmallEmpty[(int)element] = descriptor.IconSmallEmpty;

        if (descriptor.IconSmallFilled != null)
            Prefabs.Instance.ElementIconsSmallFilled[(int)element] = descriptor.IconSmallFilled;
    }
}
