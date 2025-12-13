using System.Threading.Tasks;
using Ethereal.API;

namespace Ethereal.Classes.LazyValues;

public sealed record LazyAction : LazyReferenceable<BaseAction>
{
    public LazyAction(int id)
        : base(id) { }

    public LazyAction(string name)
        : base(name) { }

    public LazyAction(BaseAction action)
        : base(action) { }

    public override async Task<BaseAction?> Get() =>
        Data
        ?? (
            Id.HasValue ? await Actions.Get(Id.Value)
            : Name is not null ? await Actions.Get(Name)
            : null
        );
}
