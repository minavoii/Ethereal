using System.Threading.Tasks;
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

    public override async Task<Buff?> Get() =>
        Data
        ?? (
            Id.HasValue ? await Buffs.Get(Id.Value)
            : Name is not null ? await Buffs.Get(Name)
            : null
        );
}
