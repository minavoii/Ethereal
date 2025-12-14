using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.API;
using Ethereal.Classes.Builders;
using Ethereal.Classes.Views;
using Ethereal.Utils.Extensions;

namespace Randomizer.API;

/// <summary>
/// A class that allows serialization and deserialization of `MonsterView` instances.
/// </summary>
internal class SerializableView()
{
    public int ID { get; set; }

    public EMonsterMainType MainType { get; set; }

    public List<EMonsterType> Types { get; set; } = [];

    public List<EElement> Elements { get; set; } = [];

    public int SignatureTrait { get; set; }

    public List<int> StartActions { get; set; } = [];

    public List<int> Perks { get; set; } = [];

    public List<(int id, List<MonsterAIActionCondition> conditions)> Scripting { get; set; } = [];

    public List<(int id, EDifficulty difficulty)> WildTraits { get; set; } = [];

    public int EliteTrait { get; set; }

    /// <summary>
    /// Create a new serializable `MonsterView`.
    /// </summary>
    /// <param name="view"></param>
    public SerializableView(MonsterView view)
        : this()
    {
        ID = view.ID;
        MainType = view.MainType;
        Types = view.Types;
        Elements = view.Elements;
        SignatureTrait = view.SignatureTrait.ID;
        StartActions = [.. view.StartActions.Select(x => x.ID)];
        Perks = [.. view.BasePerks.Select(x => x.Perk.GetComponent<Perk>().ID)];
        Scripting =
        [
            .. view.Scripting.Select(x => (x.Action.GetComponent<BaseAction>().ID, x.Conditions)),
        ];
        WildTraits =
        [
            .. view.WildTraits.Select(x => (x.Trait.GetComponent<Trait>().ID, x.MinDifficulty)),
        ];
        EliteTrait = view.EliteTrait.ID;
    }

    /// <summary>
    /// Deserialize itself into a new `MonsterView`.
    /// </summary>
    /// <returns></returns>
    public async Task<MonsterView?> Deserialize()
    {
        MonsterView? view = await Monsters.GetView(ID);

        if (view is null)
            return null;

        view.Elements = Elements;
        view.MainType = MainType;
        view.Types = Types;

        if (await Traits.Get(EliteTrait) is Trait eliteTrait)
            view.EliteTrait = eliteTrait;

        if (await Traits.Get(SignatureTrait) is Trait signatureTrait)
            view.SignatureTrait = signatureTrait;

        view.BasePerks =
        [
            .. Perks.Select(x => Data.AllPerks.Find(y => y.Perk.GetComponent<Perk>().ID == x)),
        ];

        view.Scripting =
        [
            .. (
                await Scripting.SelectAsync(async x => new MonsterAIAction()
                {
                    Action = (await Actions.Get(x.id))?.gameObject,
                    Conditions = x.conditions,
                    IsTemporary = false,
                })
            ).Where(x => x.Action is not null),
        ];

        IEnumerable<BaseAction> a = (await StartActions.SelectAsync(Actions.Get)).Where(x =>
            x is not null
        )!;

        view.StartActions =
        [
            .. (IEnumerable<BaseAction>)
                (await StartActions.SelectAsync(Actions.Get)).Where(x => x is not null),
        ];

        view.WildTraits =
        [
            .. (
                await WildTraits.SelectAsync(async x =>
                    await Traits.Get(x.id) is Trait trait
                        ? await new MonsterAITraitBuilder(trait, x.difficulty).Build()
                        : null!
                )
            ).Where(x => x is not null),
        ];

        return view;
    }
}
