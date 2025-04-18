using System.Collections.Generic;
using System.Linq;
using Ethereal.API;

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

    public List<(int, List<MonsterAIActionCondition>)> Scripting { get; set; } = [];

    public List<(int, EDifficulty)> WildTraits { get; set; } = [];

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
        Elements = descriptor.Elements;
        SignatureTrait = descriptor.SignatureTrait.ID;
        StartingActions = [.. descriptor.StartingActions.Select(x => x.ID)];
        Perks = [.. descriptor.Perks.Select(x => x.Perk.GetComponent<Perk>().ID)];
        Scripting =
        [
            .. descriptor.Scripting.Select(x =>
                (x.Action.GetComponent<BaseAction>().ID, x.Conditions)
            ),
        ];
        WildTraits =
        [
            .. descriptor.WildTraits.Select(x =>
                (x.Trait.GetComponent<Trait>().ID, x.MinDifficulty)
            ),
        ];
        EliteTrait = descriptor.EliteTrait.ID;
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
            Perks = [],
            Scripting = [],
            StartingActions = [],
            Types = Types,
            WildTraits = [],
        };

        if (Traits.TryGet(EliteTrait, out var eliteTrait))
            descriptor.EliteTrait = eliteTrait;

        if (Traits.TryGet(SignatureTrait, out var signatureTrait))
            descriptor.SignatureTrait = signatureTrait;

        foreach (int id in Perks)
        {
            PerkInfos perk = Data.allPerks.Find(x => x.Perk.GetComponent<Perk>().ID == id);

            if (perk != null)
                descriptor.Perks.Add(perk);
        }

        foreach ((int id, List<MonsterAIActionCondition> conditions) in Scripting)
        {
            if (Actions.TryGet(id, out BaseAction action))
                descriptor.Scripting.Add(
                    new() { Action = action.gameObject, Conditions = conditions }
                );
        }

        foreach (int id in StartingActions)
        {
            if (Actions.TryGet(id, out var action))
                descriptor.StartingActions.Add(action);
        }

        foreach ((int id, EDifficulty difficulty) in WildTraits)
        {
            if (Traits.TryGet(id, out var trait))
                descriptor.WildTraits.Add(
                    new() { Trait = trait.gameObject, MinDifficulty = difficulty }
                );
        }

        return descriptor;
    }
}
