using UnityEngine;

namespace Ethereal.Utils;

internal static class Assets
{
    internal static GameObject LoadAsset(string path, string asset)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(path);
        GameObject gameObject = bundle.LoadAsset<GameObject>(asset);

        if (gameObject == null)
        {
            Log.Plugin.LogError($"Could not load asset: {path}:{asset}");
            return null;
        }

        return gameObject;
    }
}
