using Ethereal.API;

namespace Ethereal.Classes.LazyValues;

public sealed record LazyAction : LazyReferenceable<BaseAction>
{
    public LazyAction(int id)
        : base(id) { }

    public LazyAction(string name)
        : base(name) { }

    public LazyAction(BaseAction action)
        : base(action) { }

    public override BaseAction? Get() =>
        base.Get()
        ?? (
            Id.HasValue && Actions.TryGet(Id.Value, out var byId) ? byId
            : Name != null && Actions.TryGet(Name, out var byName) ? byName
            : null
        );
}
