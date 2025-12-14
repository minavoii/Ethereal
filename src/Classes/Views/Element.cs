using Ethereal.Attributes;
using UnityEngine;

namespace Ethereal.Classes.Views;

/// <summary>
/// A helper class that allows easy viewing and editing of an Element.
/// </summary>
/// <param name="element"></param>
public sealed partial class ElementView(EElement element)
{
    public EElement Element => element;

    [Forward("Prefabs.Instance.ElementIcons[(int)element]")]
    public partial Sprite? Icon { get; set; }

    [Forward("Prefabs.Instance.ElementIconsSmallEmpty[(int)element]")]
    public partial Sprite? IconSmallEmpty { get; set; }

    [Forward("Prefabs.Instance.ElementIconsSmallFilled[(int)element]")]
    public partial Sprite? IconSmallFilled { get; set; }
}
