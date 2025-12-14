using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;
using Ethereal.Classes.Views;
using Ethereal.Classes.Wrappers;
using UnityEngine;

namespace Ethereal.API;

[BasicAPI]
public static partial class Traits
{
    /// <summary>
    /// Get a trait by id.
    /// </summary>
    /// <param name="id"></param>
    [GetObject, GetView(typeof(TraitView))]
    public static async Task<Trait?> Get(int id) => await Get(x => x?.ID == id);

    /// <summary>
    /// Get a trait by name.
    /// </summary>
    /// <param name="name"></param>
    [GetObject, GetView(typeof(TraitView))]
    public static async Task<Trait?> Get(string name) => await Get(x => x?.Name == name);

    /// <summary>
    /// Find a trait using a predicate from a monster type or signature trait.
    /// </summary>
    /// <param name="predicate"></param>
    [GetObject, GetView(typeof(TraitView))]
    public static async Task<Trait?> Get(Func<Trait?, bool> predicate)
    {
        await WhenReady();

        return GameController
                .Instance.MonsterTypes.SelectMany(x => x.Traits)
                .FirstOrDefault(predicate)
            ?? GameController
                .Instance.CompleteMonsterList.Select(x =>
                    x?.GetComponent<SkillManager>()?.SignatureTrait?.GetComponent<Trait>()
                )
                .FirstOrDefault(predicate)
            ?? (await Referenceables.GetManyOfType<Trait>()).FirstOrDefault(predicate);
    }

    /// <summary>
    /// Get all traits that can be learned (i.e. non-signature traits).
    /// </summary>
    public static async Task<List<Trait>> GetAllLearnable()
    {
        await WhenReady();
        return
        [
            .. GameController
                .Instance.MonsterTypes.SelectMany(x => x.Traits)
                .Where(x => x is not null)
                .Distinct(),
        ];
    }

    /// <summary>
    /// Get all signature traits.
    /// </summary>
    public static async Task<List<Trait>> GetAllSignature()
    {
        await WhenReady();
        return
        [
            .. GameController
                .Instance.CompleteMonsterList.Select(x =>
                    x?.GetComponent<SkillManager>()?.SignatureTrait?.GetComponent<Trait>()!
                )
                .Where(x => x is not null && x.Name != "?????")
                .Distinct(),
        ];
    }

    /// <summary>
    /// Get all traits, both learnable and signature ones.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<Trait>> GetAll() =>
        [.. (await GetAllLearnable()).Concat(await GetAllSignature())];

    /// <summary>
    /// Create a new trait and add it to the game's data.
    /// </summary>
    /// <param name="descriptor"></param>
    public static async Task<Trait> Add(TraitBuilder trait) => await Add(await trait.Build());

    /// <summary>
    /// Create a new trait and add it to the game's data.
    /// </summary>
    /// <param name="descriptor"></param>
    public static async Task<Trait> Add(Trait trait, bool learnable = false)
    {
        GameObject go = GameObjects.IntoGameObject(trait);
        Trait goTrait = go.GetComponent<Trait>();

        foreach (PassiveEffect passive in trait.PassiveEffectList)
        {
            if (passive is PassiveGrantBuffWrapper grantBuff)
                await grantBuff.Unwrap();
            else if (passive is PassiveGrantActionWrapper grantAction)
                await grantAction.Unwrap();

            GameObjects.CopyToGameObject(ref go, passive);
        }

        goTrait.InitializeReferenceable();
        await Referenceables.Add(goTrait);

        if (learnable)
        {
            foreach (GameObject monsterType in trait.Types)
                monsterType.GetComponent<MonsterType>().Traits.Add(goTrait);
        }

        return goTrait;
    }
}
