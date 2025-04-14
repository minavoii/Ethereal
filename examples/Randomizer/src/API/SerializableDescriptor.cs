using System.Collections.Generic;
using System.Linq;
using Ethereal.API;

namespace Randomizer.API;

/// <summary>
/// A class that allows serialization and deserialization of `MonsterDescriptor` instances.
/// </summary>
internal class SerializableDescriptor()
{
    public EMonsterMainType? mainType;

    public List<EMonsterType> types = [];

    public (EElement first, EElement second) elements = (EElement.None, EElement.None);

    public int signatureTrait;

    public List<int> startingActions = [];

    public List<int> perks = [];

    public List<(int, List<MonsterAIActionCondition>)> scripting = [];

    public List<(int, EDifficulty)> wildTraits = [];

    public int eliteTrait;

    /// <summary>
    /// Create a new serializable `MonsterDescriptor`.
    /// </summary>
    /// <param name="descriptor"></param>
    public SerializableDescriptor(Monsters.MonsterDescriptor descriptor)
        : this()
    {
        mainType = descriptor.mainType;
        types = descriptor.types;
        elements = descriptor.elements;
        signatureTrait = descriptor.signatureTrait.ID;
        startingActions = [.. descriptor.startingActions.Select(x => x.ID)];
        perks = [.. descriptor.perks.Select(x => x.Perk.GetComponent<Perk>().ID)];
        scripting =
        [
            .. descriptor.scripting.Select(x =>
                (x.Action.GetComponent<BaseAction>().ID, x.Conditions)
            ),
        ];
        wildTraits =
        [
            .. descriptor.wildTraits.Select(x =>
                (x.Trait.GetComponent<Trait>().ID, x.MinDifficulty)
            ),
        ];
        eliteTrait = descriptor.eliteTrait.ID;
    }

    /// <summary>
    /// Deserialize itself into a new `MonsterDescriptor`.
    /// </summary>
    /// <returns></returns>
    public Monsters.MonsterDescriptor Deserialize()
    {
        Monsters.MonsterDescriptor descriptor = new()
        {
            elements = elements,
            mainType = mainType,
            perks = [],
            scripting = [],
            startingActions = [],
            types = types,
            wildTraits = [],
        };

        if (Traits.TryGet(this.eliteTrait, out var eliteTrait))
            descriptor.eliteTrait = eliteTrait;

        if (Traits.TryGet(this.signatureTrait, out var signatureTrait))
            descriptor.signatureTrait = signatureTrait;

        foreach (int id in perks)
        {
            PerkInfos perk = Data.allPerks.Find(x => x.Perk.GetComponent<Perk>().ID == id);

            if (perk != null)
                descriptor.perks.Add(perk);
        }

        foreach ((int id, List<MonsterAIActionCondition> conditions) in scripting)
        {
            if (Actions.TryGet(id, out BaseAction action))
                descriptor.scripting.Add(
                    new() { Action = action.gameObject, Conditions = conditions }
                );
        }

        foreach (int id in startingActions)
        {
            if (Actions.TryGet(id, out var action))
                descriptor.startingActions.Add(action);
        }

        foreach ((int id, EDifficulty difficulty) in wildTraits)
        {
            if (Traits.TryGet(id, out var trait))
                descriptor.wildTraits.Add(
                    new() { Trait = trait.gameObject, MinDifficulty = difficulty }
                );
        }

        return descriptor;
    }
}
