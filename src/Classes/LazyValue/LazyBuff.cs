using Ethereal.API;

namespace Ethereal.Classes.LazyValues;

public sealed record LazyBuff : LazyReferenceable<Buff>
{
    public LazyBuff(int id)
        : base(id) { }

    public LazyBuff(string name)
        : base(name) { }

    public LazyBuff(Buff buff)
        : base(buff) { }

    public override Buff? Get() =>
        base.Get()
        ?? (
            Id.HasValue && Buffs.TryGet(Id.Value, out Buff byId) ? byId
            : Name is not null && Buffs.TryGet(Name, out Buff byName) ? byName
            : null
        );
}
