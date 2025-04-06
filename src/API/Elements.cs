using System.Collections.Concurrent;
using UnityEngine;

namespace Ethereal.API;

public static class Elements
{
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

    internal static void ReadQueue()
    {
        IsReady = true;

        while (QueueUpdate.TryDequeue(out var res))
            UpdateIcon(res.element, res.icons);
    }

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
