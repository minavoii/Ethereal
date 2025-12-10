using System;
using System.Collections.Generic;
using System.Linq;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;
using Ethereal.Classes.Wrappers;
using UnityEngine;

namespace Ethereal.API;

[Deferrable]
public static partial class Traits
{
    /// <summary>
    /// Get a trait by id.
    /// </summary>
    /// <param name="id"></param>
    [TryGet]
    private static Trait? Get(int id) => Get(x => x?.ID == id);

    /// <summary>
    /// Get a trait by name.
    /// </summary>
    /// <param name="name"></param>
    [TryGet]
    private static Trait? Get(string name) => Get(x => x?.Name == name);

    /// <summary>
    /// Find a trait by monster type or signature trait.
    /// </summary>
    /// <param name="predicate"></param>
    private static Trait? Get(Func<Trait?, bool> predicate) =>
        GameController.Instance.MonsterTypes.SelectMany(x => x.Traits).FirstOrDefault(predicate)
        ?? GameController
            .Instance.CompleteMonsterList.Select(x =>
                x?.GetComponent<SkillManager>()?.SignatureTrait?.GetComponent<Trait>()
            )
            .FirstOrDefault(predicate)
        ?? WorldData.Instance.Referenceables.OfType<Trait>().FirstOrDefault(predicate);

    /// <summary>
    /// Get all traits that can be learned (i.e. non-signature traits).
    /// </summary>
    [TryGet]
    private static List<Trait> GetAllLearnable() =>
        [
            .. GameController
                .Instance.MonsterTypes.SelectMany(x => x.Traits)
                .Where(x => x is not null)
                .Distinct(),
        ];

    /// <summary>
    /// Get all signature traits.
    /// </summary>
    [TryGet]
    private static List<Trait> GetAllSignature() =>
        [
            .. GameController
                .Instance.CompleteMonsterList.Select(x =>
                    x?.GetComponent<SkillManager>()?.SignatureTrait?.GetComponent<Trait>()!
                )
                .Where(x => x is not null && x.Name != "?????")
                .Distinct(),
        ];

    /// <summary>
    /// Get all traits, both learnable and signature ones.
    /// </summary>
    /// <returns></returns>
    [TryGet]
    private static List<Trait> GetAll() => [.. GetAllLearnable().Concat(GetAllSignature())];

    /// <summary>
    /// Create a new trait and add it to the game's data.
    /// </summary>
    /// <param name="descriptor"></param>
    [Deferrable]
    private static void Add_Impl(TraitBuilder trait) => Add_Impl(trait.Build());

    /// <summary>
    /// Create a new trait and add it to the game's data.
    /// </summary>
    /// <param name="descriptor"></param>
    [Deferrable]
    private static void Add_Impl(Trait trait, bool learnable = false)
    {
        GameObject go = GameObjects.IntoGameObject(trait);
        Trait goTrait = go.GetComponent<Trait>();

        foreach (PassiveEffect passive in trait.PassiveEffectList)
        {
            if (passive is PassiveGrantBuffWrapper grantBuff)
                grantBuff.Unwrap();
            else if (passive is PassiveGrantActionWrapper grantAction)
                grantAction.Unwrap();

            GameObjects.CopyToGameObject(ref go, passive);
        }

        goTrait.InitializeReferenceable();
        Referenceables.Add(goTrait);

        if (learnable)
        {
            foreach (GameObject monsterType in trait.Types)
                monsterType.GetComponent<MonsterType>().Traits.Add(goTrait);
        }
    }
}
