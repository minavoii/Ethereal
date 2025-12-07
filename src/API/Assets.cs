using UnityEngine;

namespace Ethereal.API;

public static class Assets
{
    /// <summary>
    /// Load a GameObject from an asset bundle.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static GameObject? LoadAsset(string path, string asset)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(path);
        GameObject go = bundle.LoadAsset<GameObject>(asset);

        if (go is null)
        {
            Log.Plugin.LogError($"Could not load asset: {path}:{asset}");
            return null;
        }

        return go;
    }
}
