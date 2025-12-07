using System.Linq;
using UnityEngine;

namespace Ethereal.API;

public static class Animations
{
    /// <summary>
    /// Load an animation clip from an asset bundle.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static AnimationClip? LoadFromBundle(string path, string asset) =>
        Assets.LoadBundle(path) is AssetBundle bundle ? LoadFromBundle(bundle, asset) : null;

    /// <summary>
    /// Load an animation clip from an asset bundle.
    /// </summary>
    /// <param name="bundle"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static AnimationClip? LoadFromBundle(AssetBundle bundle, string asset)
    {
        GameObject? go = Assets.LoadPrefab(bundle, asset);
        AnimationClip? clip = go
            ?.GetComponent<Animator>()
            .runtimeAnimatorController.animationClips.FirstOrDefault();

        return clip;
    }
}
