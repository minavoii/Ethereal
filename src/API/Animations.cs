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
    public static AnimationClip? LoadFromAsset(string path, string asset)
    {
        GameObject? go = Assets.LoadAsset(path, asset);
        AnimationClip? clip = go
            ?.GetComponent<Animator>()
            .runtimeAnimatorController.animationClips.FirstOrDefault();

        return clip;
    }
}
