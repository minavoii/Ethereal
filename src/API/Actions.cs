using System;
using System.Collections.Generic;
using System.Linq;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;
using Ethereal.Classes.Wrappers;
using HarmonyLib;
using UnityEngine;

namespace Ethereal.API;

[Deferrable]
public static partial class Actions
{
    /// <summary>
    /// Get an action by id.
    /// </summary>
    /// <param name="id"></param>
    [TryGet]
    private static BaseAction? Get(int id) => Get(x => x?.ID == id);

    /// <summary>
    /// Get an action by name.
    /// </summary>
    /// <param name="name"></param>
    [TryGet]
    private static BaseAction? Get(string name) => Get(x => x?.Name == name);

    private static BaseAction? Get(Func<BaseAction?, bool> predicate) =>
        GameController.Instance.MonsterTypes.SelectMany(x => x.Actions).FirstOrDefault(predicate)
        ?? WorldData.Instance.Referenceables.OfType<BaseAction>().FirstOrDefault(predicate);

    /// <summary>
    /// Get all actions.
    /// </summary>
    /// <returns></returns>
    [TryGet]
    private static List<BaseAction> GetAll() =>
        [
            .. GameController
                .Instance.MonsterTypes.SelectMany(x => x.Actions)
                .Where(x => x.Name != "?????" && x.Name != "PoiseBreaker")
                .Distinct(),
        ];

    /// <summary>
    /// Create a new action and add it to the game's data.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="modifiers"></param>
    /// <param name="learnable"></param>
    [Deferrable]
    private static void Add_Impl(
        BaseActionBuilder action,
        List<ActionModifier> modifiers,
        bool learnable = false
    ) => Add_Impl(action.Build(), modifiers, action.VFXs, learnable);

    /// <summary>
    /// Create a new action and add it to the game's data.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="modifiers"></param>
    /// <param name="learnable"></param>
    [Deferrable]
    private static void Add_Impl(
        BaseAction action,
        List<ActionModifier> modifiers,
        List<VFX.ChildVFX> vfxChildren,
        bool learnable = false
    )
    {
        GameObject go = GameObjects.IntoGameObject(action);
        BaseAction goAction = go.GetComponent<BaseAction>();
        goAction.enabled = false;

        if (action.IsFreeAction())
            goAction.SetFreeAction(true);

        foreach (ActionModifier modifier in modifiers)
        {
            if (modifier is ActionDamageWrapper damageWrapper)
                damageWrapper.Unwrap();

            GameObjects.CopyToGameObject(ref go, modifier);

            // Set the `Buffs` property here because it's private
            if (modifier is ActionApplyBuffWrapper applyBuff)
            {
                AccessTools
                    .Field(typeof(ActionApplyBuff), "Buffs")
                    .SetValue(
                        go.GetComponent<ActionApplyBuff>(),
                        applyBuff.BuffDefines.Select(x => x.Build()).ToList()
                    );
            }
        }

        if (vfxChildren.Count > 0)
        {
            VFX vfx = go.AddComponent<VFX>();
            vfx.enabled = false;
            vfx.Children = vfxChildren;
        }

        goAction.InitializeReferenceable();
        Referenceables.Add(goAction);

        if (learnable)
        {
            foreach (GameObject monsterType in action.Types)
                monsterType.GetComponent<MonsterType>().Actions.Add(goAction);
        }
    }
}
