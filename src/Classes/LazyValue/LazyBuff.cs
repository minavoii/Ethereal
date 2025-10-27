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
            Id.HasValue && Buffs.TryGet(Id.Value, out var byId) ? byId
            : Name != null && Buffs.TryGet(Name, out var byName) ? byName
            : null
        );
}
