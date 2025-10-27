using System.Collections.Generic;
using System.Linq;
using Ethereal.API;
using Ethereal.Classes.Builders;
using Ethereal.Classes.LazyValues;

namespace Randomizer.API;

/// <summary>
/// A class that allows serialization and deserialization of `MonsterDescriptor` instances.
/// </summary>
internal class SerializableDescriptor()
{
    public EMonsterMainType? MainType { get; set; }

    public List<EMonsterType> Types { get; set; } = [];

    public (EElement first, EElement second) Elements { get; set; } =
        (EElement.None, EElement.None);

    public int SignatureTrait { get; set; }

    public List<int> StartingActions { get; set; } = [];

    public List<int> Perks { get; set; } = [];

    public List<(int id, List<MonsterAIActionCondition> conditions)> Scripting { get; set; } = [];

    public List<(int id, EDifficulty difficulty)> WildTraits { get; set; } = [];

    public int EliteTrait { get; set; }

    /// <summary>
    /// Create a new serializable `MonsterDescriptor`.
    /// </summary>
    /// <param name="descriptor"></param>
    public SerializableDescriptor(Monsters.MonsterDescriptor descriptor)
        : this()
    {
        MainType = descriptor.MainType;
        Types = descriptor.Types;
        Elements = descriptor.Elements.Value;
        SignatureTrait = descriptor.SignatureTrait.Get().ID;
        StartingActions = [.. descriptor.StartingActions.Select(x => x.Get().ID)];
        Perks = [.. descriptor.Perks.Select(x => x.Build().Perk.GetComponent<Perk>().ID)];
        Scripting =
        [
            .. descriptor.Scripting.Select(x =>
                (x.Action.Get().GetComponent<BaseAction>().ID, x.Conditions)
            ),
        ];
        WildTraits =
        [
            .. descriptor.WildTraits.Select(x =>
                (x.Trait.Get().GetComponent<Trait>().ID, x.MinDifficulty)
            ),
        ];
        EliteTrait = descriptor.EliteTrait.Get().ID;
    }

    /// <summary>
    /// Deserialize itself into a new `MonsterDescriptor`.
    /// </summary>
    /// <returns></returns>
    public Monsters.MonsterDescriptor Deserialize()
    {
        Monsters.MonsterDescriptor descriptor = new()
        {
            Elements = Elements,
            MainType = MainType,
            Types = Types,
            EliteTrait = Traits.TryGet(EliteTrait, out Trait eliteTrait) ? new(eliteTrait) : null,
            SignatureTrait = Traits.TryGet(SignatureTrait, out Trait signatureTrait)
                ? new(signatureTrait)
                : null,
            Perks =
            [
                .. Perks.Select(x => new PerkInfosBuilder(
                    Data.AllPerks.Find(y => y.Perk.GetComponent<Perk>().ID == x)
                )),
            ],
            Scripting =
            [
                .. Scripting
                    .Select(x =>
                        Actions.TryGet(x.id, out BaseAction action)
                            ? new MonsterAIActionBuilder(action, x.conditions)
                            : null
                    )
                    .Where(x => x is not null),
            ],
            StartingActions =
            [
                .. StartingActions
                    .Select(x =>
                        Actions.TryGet(x, out BaseAction action) ? new LazyAction(action) : null
                    )
                    .Where(x => x is not null),
            ],
            WildTraits =
            [
                .. WildTraits
                    .Select(x =>
                        Traits.TryGet(x.id, out Trait trait)
                            ? new MonsterAITraitBuilder(trait, x.difficulty)
                            : null
                    )
                    .Where(x => x is not null),
            ],
        };

        return descriptor;
    }
}
