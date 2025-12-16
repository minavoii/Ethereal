using System.Threading.Tasks;
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

    public override async Task<Perk?> Get() =>
        Data
        ?? (
            Id.HasValue ? await Perks.Get(Id.Value)
            : Name is not null ? await Perks.Get(Name)
            : null
        );
}
