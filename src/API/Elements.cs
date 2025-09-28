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

    private static readonly QueueableAPI API = new();

    internal static void SetReady() => API.SetReady();

    /// <summary>
    /// Set an element's icon.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="descriptor"></param>
    public static void Update(EElement element, ElementDescriptor descriptor)
    {
        // Defer loading until ready
        if (!API.IsReady)
        {
            API.Enqueue(() => Update(element, descriptor));
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
