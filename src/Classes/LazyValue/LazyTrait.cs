using System.Threading.Tasks;
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

    public override async Task<Trait?> Get() =>
        Data
        ?? (
            Id.HasValue ? await Traits.Get(Id.Value)
            : Name is not null ? await Traits.Get(Name)
            : null
        );
}
