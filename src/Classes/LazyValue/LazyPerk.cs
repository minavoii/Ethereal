using Ethereal.API;

namespace Ethereal.Classes.LazyValues;

public sealed record LazyPerk : LazyReferenceable<Perk>
{
    public LazyPerk(int id)
        : base(id) { }

    public LazyPerk(string name)
        : base(name) { }

    public LazyPerk(Perk perk)
        : base(perk) { }

    public override Perk? Get() =>
        base.Get()
        ?? (
            Id.HasValue && Perks.TryGet(Id.Value, out Perk byId) ? byId
            : Name != null && Perks.TryGet(Name, out Perk byName) ? byName
            : null
        );
}
