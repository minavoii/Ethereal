using UnityEngine;

namespace Ethereal.API;

public static class Assets
{
    /// <summary>
    /// Load an AssetBundle from disk.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static AssetBundle? LoadBundle(string path) => AssetBundle.LoadFromFile(path);

    /// <summary>
    /// Load a GameObject from an asset bundle.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static GameObject? LoadPrefab(string path, string asset) =>
        LoadBundle(path) is AssetBundle bundle ? LoadPrefab(bundle, asset) : null;

    /// <summary>
    /// Load a GameObject from an asset bundle.
    /// </summary>
    /// <param name="bundle"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static GameObject? LoadPrefab(AssetBundle bundle, string asset)
    {
        GameObject go = bundle.LoadAsset<GameObject>(asset);

        if (go is null)
        {
            Log.API.LogError($"Could not load asset: {asset}");
            return null;
        }

        return go;
    }

    /// <summary>
    /// Load a GameObject from an asset bundle.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static T? LoadAsset<T>(string path, string asset)
        where T : Object =>
        LoadBundle(path) is AssetBundle bundle ? LoadAsset<T>(bundle, asset) : null;

    /// <summary>
    /// Load a GameObject from an asset bundle.
    /// </summary>
    /// <param name="bundle"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static T? LoadAsset<T>(AssetBundle bundle, string asset)
        where T : Object
    {
        T component = bundle.LoadAsset<T>(asset);

        if (component is null)
        {
            Log.API.LogError($"Could not load asset: {asset}");
            return null;
        }

        return component;
    }
}
