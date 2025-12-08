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
            Id.HasValue && Actions.TryGet(Id.Value, out BaseAction byId) ? byId
            : Name is not null && Actions.TryGet(Name, out BaseAction byName) ? byName
            : null
        );
}
