using Ethereal.API;
using HarmonyLib;
using UnityEngine;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that stores a Monster's bounds.
/// </summary>
/// <param name="Bounds"></param>
/// <param name="BoundsOffset"></param>
/// <param name="FocusPoint"></param>
public sealed record MonsterBounds(Vector2? Bounds, Vector2? BoundsOffset, Vector3? FocusPoint);

/// <summary>
/// A helper record that creates a Monster at runtime.
/// </summary>
/// <param name="Monster"></param>
/// <param name="Animator"></param>
/// <param name="Bounds"></param>
/// <param name="SkillManager"></param>
/// <param name="Stats"></param>
/// <param name="AI"></param>
/// <param name="OverworldBehaviour"></param>
/// <param name="Shift"></param>
public sealed record MonsterBuilder(
    Monster Monster,
    MonsterAnimator Animator,
    MonsterBounds Bounds,
    SkillManagerBuilder SkillManager,
    MonsterStatsBuilder Stats,
    MonsterAIBuilder AI,
    OverworldMonsterBehaviour OverworldBehaviour,
    MonsterShiftBuilder Shift
)
{
    public GameObject Build()
    {
        GameObject go = new();
        Monster goMonster = go.GetComponent<Monster>();

        Monster.Animator = Animator;

        GameObjects.CopyToGameObject(ref go, Animator);
        go.AddComponent<SpriteRenderer>();
        GameObjects.CopyToGameObject(ref go, OverworldBehaviour);
        GameObjects.CopyToGameObject(ref go, SkillManager.Build());
        GameObjects.CopyToGameObject(ref go, Stats.Build());
        GameObjects.CopyToGameObject(ref go, AI.Build());
        GameObjects.CopyToGameObject(ref go, Monster);
        GameObjects.CopyToGameObject(ref go, Shift.Build());
        GameObject.Destroy(go.GetComponent<SpriteAnim>());

        if (Bounds.Bounds != null)
        {
            AccessTools.Field(typeof(Monster), "Bounds").SetValue(goMonster, Bounds.Bounds);
        }
        if (Bounds.BoundsOffset != null)
        {
            AccessTools
                .Field(typeof(Monster), "BoundsOffset")
                .SetValue(goMonster, Bounds.BoundsOffset);
        }

        Transform focusPointTransform = new GameObject("FocusPoint").transform;

        if (Bounds.FocusPoint is Vector3 focusPoint)
            focusPointTransform.position = focusPoint;

        AccessTools.Field(typeof(Monster), "FocusPoint").SetValue(goMonster, focusPointTransform);

        return go;
    }
}
