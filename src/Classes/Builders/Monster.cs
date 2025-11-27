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
        GameObject monster_go = new();
        Monster.Animator = Animator;
        Utils.GameObjects.CopyToGameObject(ref monster_go, Monster);

        LateReferenceables.Queue(() =>
        {
            Debug.Log($"Setting up Monster {Monster.Name} in late update");
            Utils.GameObjects.CopyToGameObject(ref monster_go, Animator);
            monster_go.AddComponent<SpriteRenderer>();
            Utils.GameObjects.CopyToGameObject(ref monster_go, OverworldBehaviour);
            Utils.GameObjects.CopyToGameObject(ref monster_go, SkillManager.Build());
            Utils.GameObjects.CopyToGameObject(ref monster_go, Stats.Build());
            Utils.GameObjects.CopyToGameObject(ref monster_go, AI.Build());
            Utils.GameObjects.CopyToGameObject(ref monster_go, Shift.Build());
            GameObject.Destroy(monster_go.GetComponent<SpriteAnim>());

            if (Bounds.Bounds != null)
            {
                AccessTools
                    .Field(typeof(Monster), "Bounds")
                    .SetValue(monster_go.GetComponent<Monster>(), Bounds.Bounds);
            }
            if (Bounds.BoundsOffset != null)
            {
                AccessTools
                    .Field(typeof(Monster), "BoundsOffset")
                    .SetValue(monster_go.GetComponent<Monster>(), Bounds.BoundsOffset);
            }

            Transform focusPointTransform = new GameObject("FocusPoint").transform;

            if (Bounds.FocusPoint is Vector3 focusPoint)
                focusPointTransform.position = focusPoint;

            AccessTools
                .Field(typeof(Monster), "FocusPoint")
                .SetValue(monster_go.GetComponent<Monster>(), focusPointTransform);
        });

        return monster_go;
    }
}
