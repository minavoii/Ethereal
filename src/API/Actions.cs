using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;
using Ethereal.Classes.Wrappers;
using HarmonyLib;
using UnityEngine;

namespace Ethereal.API;

[BasicAPI]
public static partial class Actions
{
    /// <summary>
    /// Get an action by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static async Task<BaseAction?> Get(int id) => await Get(x => x?.ID == id);

    /// <summary>
    /// Get an action by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static async Task<BaseAction?> Get(string name) => await Get(x => x?.Name == name);

    /// <summary>
    /// Get an action using a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static async Task<BaseAction?> Get(Func<BaseAction?, bool> predicate)
    {
        await API.WhenReady();

        return GameController
                .Instance.MonsterTypes.SelectMany(x => x.Actions)
                .FirstOrDefault(predicate)
            ?? (await Referenceables.GetManyOfType<BaseAction>()).FirstOrDefault(predicate);
    }

    /// <summary>
    /// Get all actions.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<BaseAction>> GetAll()
    {
        await API.WhenReady();

        return
        [
            .. GameController
                .Instance.MonsterTypes.SelectMany(x => x.Actions)
                .Where(x => x.Name != "?????" && x.Name != "PoiseBreaker")
                .Distinct(),
        ];
    }

    /// <summary>
    /// Create a new action and add it to the game's data.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="modifiers"></param>
    /// <param name="learnable"></param>
    public static async Task<BaseAction> Add(
        BaseActionBuilder action,
        List<ActionModifier> modifiers,
        bool learnable = false
    ) => await Add(await action.Build(), modifiers, action.VFXs, learnable);

    /// <summary>
    /// Create a new action and add it to the game's data.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="modifiers"></param>
    /// <param name="learnable"></param>
    public static async Task<BaseAction> Add(
        BaseAction action,
        List<ActionModifier> modifiers,
        List<VFX.ChildVFX> vfxChildren,
        bool learnable = false
    )
    {
        await API.WhenReady();

        GameObject go = GameObjects.IntoGameObject(action);
        BaseAction goAction = go.GetComponent<BaseAction>();
        goAction.enabled = false;

        if (action.IsFreeAction())
            goAction.SetFreeAction(true);

        foreach (ActionModifier modifier in modifiers)
        {
            if (modifier is ActionDamageWrapper damageWrapper)
                await damageWrapper.Unwrap();

            GameObjects.CopyToGameObject(ref go, modifier);

            // Set the `Buffs` property here because it's private
            if (modifier is ActionApplyBuffWrapper applyBuff)
            {
                AccessTools
                    .Field(typeof(ActionApplyBuff), "Buffs")
                    .SetValue(
                        go.GetComponent<ActionApplyBuff>(),
                        (List<ActionApplyBuff.BuffDefine>)
                            [
                                .. await System.Threading.Tasks.Task.WhenAll(
                                    applyBuff.BuffDefines.Select(async x => await x.Build())
                                ),
                            ]
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
        await Referenceables.Add(goAction);

        if (learnable)
        {
            foreach (GameObject monsterType in action.Types)
                monsterType.GetComponent<MonsterType>().Actions.Add(goAction);
        }

        return goAction;
    }
}
