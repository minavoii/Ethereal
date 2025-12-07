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
}
