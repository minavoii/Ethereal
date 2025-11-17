using UnityEngine;

namespace Ethereal.API;

public static class VFXs
{
    public static GameObject CreateCosmetic(AnimationClip animation)
    {
        GameObject go = new();
        VFX vfx = go.AddComponent<VFX>();
        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
        CosmeticVfxAnimator vfxAnimator = go.AddComponent<CosmeticVfxAnimator>();

        vfx.enabled = false;
        spriteRenderer.forceRenderingOff = true;
        spriteRenderer.sortingOrder = 1;
        vfxAnimator.Animation = animation;

        return go;
    }
}
