using System.Linq;
using Ethereal.Classes.Exceptions;
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
    public static AnimationClip LoadFromBundle(string path, string asset) =>
        LoadFromBundle(Assets.LoadBundle(path), asset);

    /// <summary>
    /// Load an animation clip from an asset bundle.
    /// </summary>
    /// <param name="bundle"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static AnimationClip LoadFromBundle(AssetBundle bundle, string asset) =>
        Assets
            .LoadPrefab(bundle, asset)
            .GetComponent<Animator>()
            ?.runtimeAnimatorController.animationClips?.FirstOrDefault()
        ?? throw new AssetNotFoundException($"AnimationClip not found in asset: {asset}");
}
