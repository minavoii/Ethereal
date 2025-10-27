using Ethereal.Attributes;
using UnityEngine;

namespace Ethereal.API;

[Deferrable]
public static partial class Elements
{
    /// <summary>
    /// A helper class that describes an element's properties.
    /// </summary>
    public class ElementDescriptor()
    {
        public Sprite? Icon { get; set; }

        public Sprite? IconSmall { get; set; }

        public Sprite? IconSmallEmpty { get; set; }

        public Sprite? IconSmallFilled { get; set; }
    }

    /// <summary>
    /// Set an element's icon.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="descriptor"></param>
    [Deferrable]
    private static void Update_Impl(EElement element, ElementDescriptor descriptor)
    {
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
