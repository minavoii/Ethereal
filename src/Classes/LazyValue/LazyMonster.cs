using System.Threading.Tasks;
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

    public override async Task<Monster?> Get() =>
        Data
        ?? (
            Id.HasValue ? await Monsters.Get(Id.Value)
            : Name is not null ? await Monsters.Get(Name)
            : null
        );
}
