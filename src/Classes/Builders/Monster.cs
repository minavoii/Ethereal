using Ethereal.API;
using Ethereal.CustomFlags;
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
/// <param name="Monster">Monster definition</param>
/// <param name="Sprite">The default sprite of the monster</param> 
/// <param name="Animator">Defines all monster animations</param>
/// <param name="Bounds">Defines the bounds of the monster; used for positioning</param>
/// <param name="SkillManager">Defines all skills of the monster</param>
/// <param name="Stats">Defines monster stats</param>
/// <param name="AI">Defines the AI behavior for enemy monsters</param>
/// <param name="OverworldBehaviour">Defines the overworld behavior</param>
/// <param name="Shift">Defines monster shift overrides</param>
public sealed record MonsterBuilder(
    Monster Monster,
    Sprite Sprite,
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
        monster_go.name = $"Monster{Monster.Name.Replace(" ", "")}";
        monster_go.AddCustomTag();
        Monster.Animator = Animator;
        Utils.GameObjects.CopyToGameObject(ref monster_go, Monster);
        if (Monster.Projectile != null)
        {
            Monster.Projectile.transform.SetParent(monster_go.transform);
        }

        LateReferenceables.Queue(() =>
        {
            Debug.Log($"Setting up Monster {Monster.Name} in late update");
            Utils.GameObjects.CopyToGameObject(ref monster_go, Animator);
            SpriteRenderer sr = monster_go.AddComponent<SpriteRenderer>();
            sr.sprite = Sprite;
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

            GameObject focusObj = new GameObject();
            focusObj.transform.SetParent(monster_go.transform);
            Transform focusPointTransform = focusObj.transform;

            if (Bounds.FocusPoint is Vector3 focusPoint)
                focusPointTransform.position = focusPoint;

            AccessTools
                .Field(typeof(Monster), "FocusPoint")
                .SetValue(monster_go.GetComponent<Monster>(), focusPointTransform);

            monster_go.SetActive(false);
        });

        return monster_go;
    }
}
