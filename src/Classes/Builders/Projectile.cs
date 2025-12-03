using HarmonyLib;
using UnityEngine;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a Projectile at runtime.
/// </summary>
public sealed record ProjectileBuilder(
    Projectile Projectile,
    Sprite? DefaultSprite,
    AnimationClip DefaultAnim
)
{
    public GameObject Build()
    {
        GameObject projectile_go = new();

        SpriteRenderer sr = projectile_go.AddComponent<SpriteRenderer>();
        if (DefaultSprite != null)
        {
            sr.sprite = DefaultSprite;
        }
        projectile_go.AddComponent<Animator>();
        SpriteAnim sa = projectile_go.AddComponent<SpriteAnim>();
        if (DefaultAnim != null)
        {
            AccessTools
                .Field(typeof(SpriteAnim), "m_defaultAnim")
                .SetValue(sa, DefaultAnim);
        }

        Utils.GameObjects.CopyToGameObject(ref projectile_go, Projectile);

        projectile_go.SetActive(false);

        return projectile_go;
    }
}
