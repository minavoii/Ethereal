using Ethereal.API;

namespace Ethereal.Classes.LazyValues;

public sealed record LazyMonster : LazyReferenceable<Monster>
{
    public LazyMonster(int id)
        : base(id) { }

    public LazyMonster(string name)
        : base(name) { }

    public LazyMonster(Monster monster)
        : base(monster) { }

    public override Monster? Get() =>
        base.Get()
        ?? (
            Id.HasValue && Monsters.TryGet(Id.Value, out Monster byId) ? byId
            : Name != null && Monsters.TryGet(Name, out Monster byName) ? byName
            : null
        );
}
