using Ethereal.Attributes;
using Ethereal.Classes.LazyValues;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a MonsterAITrait at runtime.
/// </summary>
[PrivatePrimaryConstructor]
public sealed partial record MonsterAITraitBuilder
{
    [LazyValueConstructor]
    public MonsterAITraitBuilder(
        Trait trait,
        EDifficulty minDifficulty,
        EMonsterShift? shiftRestriction = null
    )
        : this(new LazyTrait(trait), minDifficulty, shiftRestriction) { }

    public LazyTrait Trait { get; init; }

    public EDifficulty MinDifficulty { get; init; }

    public EMonsterShift? ShiftRestriction { get; init; }

    public MonsterAI.MonsterAITrait Build() =>
        new()
        {
            Trait = Trait.Get()?.gameObject,
            MinDifficulty = MinDifficulty,
            HasShiftRestriction = ShiftRestriction.HasValue,
            ShiftRestriction = ShiftRestriction ?? EMonsterShift.Normal,
        };
}
