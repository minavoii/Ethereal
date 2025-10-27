using Ethereal.API;

namespace Ethereal.Classes.LazyValues;

public sealed partial record LazyTrait : LazyReferenceable<Trait>
{
    public LazyTrait(int id)
        : base(id) { }

    public LazyTrait(string name)
        : base(name) { }

    public LazyTrait(Trait trait)
        : base(trait) { }

    public override Trait? Get() =>
        base.Get()
        ?? (
            Id.HasValue && Traits.TryGet(Id.Value, out var byId) ? byId
            : Name != null && Traits.TryGet(Name, out var byName) ? byName
            : null
        );
}
