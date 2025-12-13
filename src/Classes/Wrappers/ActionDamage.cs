using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Classes.LazyValues;

namespace Ethereal.Classes.Wrappers;

public sealed class ActionDamageWrapper(
    List<LazyBuff> buffsToCheck,
    List<LazyBuff>? secondaryBuffsToCheck = null
) : ActionDamage
{
    public List<LazyBuff> BuffsToCheckWrapper { get; init; } = buffsToCheck;

    public List<LazyBuff>? SecondaryBuffsToCheckWrapper { get; init; } = secondaryBuffsToCheck;

    public async Task Unwrap()
    {
        BuffsToCheck =
        [
            .. await Task.WhenAll(
                BuffsToCheckWrapper.Select(x => x.Get()!).Where(x => x is not null)
            ),
        ];

        if (SecondaryBuffsToCheckWrapper is not null)
            SecondaryBuffsToCheck =
            [
                .. await Task.WhenAll(
                    SecondaryBuffsToCheckWrapper.Select(x => x.Get()!).Where(x => x is not null)
                ),
            ];
    }
}
