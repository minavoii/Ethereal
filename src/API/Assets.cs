using Ethereal.Classes.Exceptions;
using UnityEngine;

namespace Ethereal.API;

public static class Assets
{
    /// <summary>
    /// Load an AssetBundle from disk.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static AssetBundle LoadBundle(string path) =>
        AssetBundle.LoadFromFile(path)
        ?? throw new AssetNotFoundException($"AssetBundle not found: {path}");

    /// <summary>
    /// Load a GameObject from an asset bundle.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static GameObject LoadPrefab(string path, string asset) =>
        LoadPrefab(LoadBundle(path), asset);

    /// <summary>
    /// Load a GameObject from an asset bundle.
    /// </summary>
    /// <param name="bundle"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static GameObject LoadPrefab(AssetBundle bundle, string asset) =>
        LoadAsset<GameObject>(bundle, asset);

    /// <summary>
    /// Load an asset from an asset bundle.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static T LoadAsset<T>(string path, string asset)
        where T : Object => LoadAsset<T>(LoadBundle(path), asset);

    /// <summary>
    /// Load an asset from an asset bundle.
    /// </summary>
    /// <param name="bundle"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static T LoadAsset<T>(AssetBundle bundle, string asset)
        where T : Object =>
        bundle.LoadAsset<T>(asset) ?? throw new AssetNotFoundException($"Asset not found: {asset}");
}
