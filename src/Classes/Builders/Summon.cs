using Ethereal.API;
using HarmonyLib;
using UnityEngine;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a Summon at runtime.
/// </summary>
/// <param name="Summon"></param>
/// <param name="Animator"></param>
/// <param name="Bounds"></param>
/// <param name="SkillManager"></param>
/// <param name="Stats"></param>
/// <param name="AI"></param>
public sealed record SummonBuilder(
    Summon Summon,
    MonsterAnimator Animator,
    MonsterBounds Bounds,
    SkillManagerBuilder SkillManager,
    SummonStatsBuilder Stats,
    MonsterAIBuilder AI
)
{
    public GameObject Build()
    {
        GameObject summon_go = new();
        Summon.Animator = Animator;
        Utils.GameObjects.CopyToGameObject(ref summon_go, Summon);

        LateReferenceables.Queue(() =>
        {
            Debug.Log($"Setting up Summon {Summon.Name} in late update");
            Utils.GameObjects.CopyToGameObject(ref summon_go, Animator);
            summon_go.AddComponent<SpriteRenderer>();
            Utils.GameObjects.CopyToGameObject(ref summon_go, SkillManager.Build());
            Utils.GameObjects.CopyToGameObject(ref summon_go, Stats.Build());
            Utils.GameObjects.CopyToGameObject(ref summon_go, AI.Build());
            GameObject.Destroy(summon_go.GetComponent<SpriteAnim>());

            if (Bounds.Bounds != null)
            {
                AccessTools
                    .Field(typeof(Summon), "Bounds")
                    .SetValue(summon_go.GetComponent<Summon>(), Bounds.Bounds);
            }
            if (Bounds.BoundsOffset != null)
            {
                AccessTools
                    .Field(typeof(Summon), "BoundsOffset")
                    .SetValue(summon_go.GetComponent<Summon>(), Bounds.BoundsOffset);
            }

            Transform focusPointTransform = new GameObject("FocusPoint").transform;

            if (Bounds.FocusPoint is Vector3 focusPoint)
                focusPointTransform.position = focusPoint;

            AccessTools
                .Field(typeof(Summon), "FocusPoint")
                .SetValue(summon_go.GetComponent<Summon>(), focusPointTransform);
        });

        return summon_go;
    }
}
