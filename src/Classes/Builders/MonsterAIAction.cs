using System.Collections.Generic;
using System.Threading.Tasks;
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

    public async Task<MonsterAIAction> Build() =>
        new()
        {
            Action = await Action.GetObject(),
            Conditions = Conditions,
            IsTemporary = IsTemporary,
        };
}
