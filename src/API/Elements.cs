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

        Prefabs.Instance.ElementIcons[(int)element] = icons.icon;
        Prefabs.Instance.ElementIconsSmall[(int)element] = icons.iconSmall;
        Prefabs.Instance.ElementIconsSmallEmpty[(int)element] = icons.iconSmallEmpty;
        Prefabs.Instance.ElementIconsSmallFilled[(int)element] = icons.iconSmallFilled;
    }
}
