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
            Plugin.Log.LogError("Could not load asset.");
            return null;
        }

        return gameObject;
    }
}
