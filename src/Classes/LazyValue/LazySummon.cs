using Ethereal.API;

namespace Ethereal.Classes.LazyValues;

public sealed record LazySummon : LazyReferenceable<Summon>
{
    public LazySummon(int id)
        : base(id) { }

    public LazySummon(string name)
        : base(name) { }

    public LazySummon(Summon summon)
        : base(summon) { }

    public override Summon? Get() =>
        base.Get()
        ?? (
            Id.HasValue && Summons.TryGet(Id.Value, out var byId) ? byId
            : Name != null && Summons.TryGet(Name, out var byName) ? byName
            : null
        );
}
