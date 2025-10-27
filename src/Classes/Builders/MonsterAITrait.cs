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
    public MonsterAITraitBuilder(Trait trait, EDifficulty minDifficulty)
        : this(new(trait), minDifficulty, false) { }

    [LazyValueConstructor]
    public MonsterAITraitBuilder(
        Trait trait,
        EDifficulty minDifficulty,
        EMonsterShift shiftRestriction
    )
        : this(new(trait), minDifficulty, true, shiftRestriction) { }

    public LazyTrait Trait { get; init; }

    public EDifficulty MinDifficulty { get; init; }

    public bool HasShiftRestriction { get; init; }

    public EMonsterShift? ShiftRestriction { get; init; }

    public MonsterAI.MonsterAITrait Build() =>
        new()
        {
            Trait = Trait.Get()?.gameObject,
            MinDifficulty = MinDifficulty,
            HasShiftRestriction = HasShiftRestriction,
            ShiftRestriction = HasShiftRestriction ? EMonsterShift.Normal : default,
        };
}
