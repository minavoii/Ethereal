using Ethereal.Classes.LazyValues;

namespace Ethereal.Classes.Wrappers;

public sealed class ActionSummonWrapper(
    LazySummon summon
) : ActionSummon
{
    public LazySummon SummonWrapper { get; init; } = summon;

    public void Unwrap()
    {
        Summon = SummonWrapper.Get()?.gameObject;
    }
}
