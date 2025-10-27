using Ethereal.Classes.LazyValues;

namespace Ethereal.Classes.Wrappers;

public sealed class PassiveGrantBuffWrapper(LazyBuff grantBuff) : PassiveGrantBuff
{
    public LazyBuff GrantBuffWrapper { get; init; } = grantBuff;

    public void Unwrap() => GrantBuff = GrantBuffWrapper.Get()?.gameObject;
}
