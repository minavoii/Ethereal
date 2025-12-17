using System.IO;
using Ethereal.Classes.Exceptions;
using UnityEngine;

namespace Ethereal.API;

public static class Textures
{
    /// <summary>
    /// Create an empty texture.
    /// </summary>
    /// <returns></returns>
    public static Texture2D Create() =>
        new(1, 1, TextureFormat.ARGB32, false) { filterMode = FilterMode.Point };

    /// <summary>
    /// Create a texture from an image file.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="asset"></param>
    /// <returns>a Texture2D if the file was found; otherwise null.</returns>
    public static Texture2D LoadFromImage(string path)
    {
        Texture2D texture = Create();
        texture.LoadImage(File.ReadAllBytes(path));

        return texture;
    }

    /// <summary>
    /// Load a texture from an asset bundle.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static Texture2D LoadFromBundle(string path, string asset) =>
        LoadFromBundle(Assets.LoadBundle(path), asset);

    /// <summary>
    /// Load a texture from an asset bundle.
    /// </summary>
    /// <param name="bundle"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static Texture2D LoadFromBundle(AssetBundle bundle, string asset) =>
        Assets.LoadPrefab(bundle, asset)?.GetComponent<Texture2D>()
        ?? throw new AssetNotFoundException($"Texture not found in asset: {asset}");
}
