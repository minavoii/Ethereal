using System.Collections.Concurrent;
using UnityEngine;

namespace Ethereal.API;

public static class Elements
{
    /// <summary>
    /// A helper class that describes an element's properties.
    /// </summary>
    public class ElementIcons()
    {
        public Sprite icon;

        public Sprite iconSmall;

        public Sprite iconSmallEmpty;

        public Sprite iconSmallFilled;
    }

    private static readonly ConcurrentQueue<(EElement element, ElementIcons icons)> QueueUpdate =
        new();

    private static bool IsReady = false;

    /// <summary>
    /// Mark the API as ready and run all deferred methods.
    /// </summary>
    internal static void SetReady()
    {
        IsReady = true;

        while (QueueUpdate.TryDequeue(out var item))
            UpdateIcon(item.element, item.icons);

        while (QueueUpdate.TryDequeue(out var item))
            UpdateIcon(item.element, item.icons);
    }

    /// <summary>
    /// Set an element's icon.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="icons"></param>
    public static void UpdateIcon(EElement element, ElementIcons icons)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdate.Enqueue((element, icons));
            return;
        }

        if (icons.icon != null)
            Prefabs.Instance.ElementIcons[(int)element] = icons.icon;

        if (icons.iconSmall != null)
            Prefabs.Instance.ElementIconsSmall[(int)element] = icons.iconSmall;

        if (icons.iconSmallEmpty != null)
            Prefabs.Instance.ElementIconsSmallEmpty[(int)element] = icons.iconSmallEmpty;

        if (icons.iconSmallFilled != null)
            Prefabs.Instance.ElementIconsSmallFilled[(int)element] = icons.iconSmallFilled;
    }
}
