using System.Threading.Tasks;
using Ethereal.Attributes;
using Ethereal.Classes.View;

namespace Ethereal.API;

[BasicAPI]
public static partial class Elements
{
    public static async Task<ElementView> GetView(EElement element)
    {
        await WhenReady();
        return new(element);
    }
}
