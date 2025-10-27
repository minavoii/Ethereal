using System.Collections.Generic;
using System.Linq;
using Ethereal.Classes.LazyValues;

namespace Ethereal.Classes.Wrappers;

public sealed class ActionDamageWrapper(
    List<LazyBuff> buffsToCheck,
    List<LazyBuff>? secondaryBuffsToCheck = null
) : ActionDamage
{
    public List<LazyBuff> BuffsToCheckWrapper { get; init; } = buffsToCheck;

    public List<LazyBuff>? SecondaryBuffsToCheckWrapper { get; init; } = secondaryBuffsToCheck;

    public void Unwrap()
    {
        BuffsToCheck = [.. BuffsToCheckWrapper.Select(x => x.Get()!).Where(x => x is not null)];

        if (SecondaryBuffsToCheckWrapper is not null)
            SecondaryBuffsToCheck =
            [
                .. SecondaryBuffsToCheckWrapper.Select(x => x.Get()!).Where(x => x is not null),
            ];
    }
}
