using System.Threading.Tasks;
using Ethereal.Classes.LazyValues;

namespace Ethereal.Classes.Wrappers;

public sealed class PassiveGrantBuffWrapper(LazyBuff grantBuff) : PassiveGrantBuff
{
    public LazyBuff GrantBuffWrapper { get; init; } = grantBuff;

    public async Task Unwrap() => GrantBuff = (await GrantBuffWrapper.Get())?.gameObject;
}
