using System.IO;
using UnityEngine;

namespace Ethereal.API;

public static partial class Textures
{
    /// <summary>
    /// Create a texture from an image file.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="asset"></param>
    /// <returns>a Texture2D if the file was found; otherwise null.</returns>
    public static Texture2D? LoadFromImage(string path)
    {
        if (!File.Exists(path))
        {
            Log.API.LogError("File not found: " + path);
            return null;
        }

        Texture2D texture = new(0, 0);
        texture.LoadImage(File.ReadAllBytes(path));

        return texture;
    }

    /// <summary>
    /// Load a texture from an asset bundle.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static Texture2D? LoadFromBundle(string path, string asset) =>
        Assets.LoadBundle(path) is AssetBundle bundle ? LoadFromBundle(bundle, asset) : null;

    /// <summary>
    /// Load a texture from an asset bundle.
    /// </summary>
    /// <param name="bundle"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static Texture2D? LoadFromBundle(AssetBundle bundle, string asset)
    {
        GameObject? go = Assets.LoadPrefab(bundle, asset);
        Texture2D? texture = go?.GetComponent<Texture2D>();

        return texture;
    }
}
