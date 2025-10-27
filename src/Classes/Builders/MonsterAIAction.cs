using System.Collections.Generic;
using Ethereal.Attributes;
using Ethereal.Classes.LazyValues;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a MonsterAIAction at runtime.
/// </summary>
[PrivatePrimaryConstructor]
public sealed partial record MonsterAIActionBuilder
{
    [LazyValueConstructor]
    public MonsterAIActionBuilder(
        BaseAction action,
        List<MonsterAIActionCondition> conditions,
        bool isTemporary = false
    )
        : this(new LazyAction(action), conditions, isTemporary) { }

    public LazyAction Action { get; init; }

    public List<MonsterAIActionCondition> Conditions { get; init; }

    public bool IsTemporary { get; init; }

    public MonsterAIAction Build() =>
        new()
        {
            Action = Action.Get()?.gameObject,
            Conditions = Conditions,
            IsTemporary = IsTemporary,
        };
}
