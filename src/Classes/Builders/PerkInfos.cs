using Ethereal.Attributes;
using Ethereal.Classes.LazyValues;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a PerkInfos at runtime.
/// </summary>
[PrivatePrimaryConstructor]
public sealed partial record PerkInfosBuilder
{
    public PerkInfosBuilder(int id, float multiplier)
        : this(new LazyPerk(id), multiplier) { }

    public PerkInfosBuilder(string name, float multiplier)
        : this(new LazyPerk(name), multiplier) { }

    public PerkInfosBuilder(PerkInfos perkInfos)
        : this(new LazyPerk(perkInfos.Perk.GetComponent<Perk>()), perkInfos.Multiplier) { }

    public LazyPerk LazyPerk { get; init; }

    public float Multiplier { get; init; }

    public PerkInfos? Build() =>
        new() { Perk = LazyPerk.Get()?.gameObject, Multiplier = Multiplier };
}
