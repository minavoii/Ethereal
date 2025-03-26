using UnityEngine;

namespace Ethereal.API;

public static class Icon
{
    public enum IconType
    {
        Equipment,
    }

    public static Sprite LoadFromAsset(string path, string asset)
    {
        GameObject gameObject = Utils.Assets.LoadAsset(path, asset);
        Sprite sprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        return sprite;
    }

    public static Sprite LoadFromImage(IconType iconType, string path, string iconName)
    {
        if (!System.IO.File.Exists(path))
        {
            API2.Log.LogWarning("File not found: " + path);
            return null;
        }

        Sprite sprite = null;

        if (iconType == IconType.Equipment)
            sprite = GetEquipment("Equipment_" + iconName.Replace(" ", ""));

        sprite.texture.LoadImage(System.IO.File.ReadAllBytes(path));

        return sprite;
    }

    private static Sprite GetEquipment(string name)
    {
        Texture2D texture = new(48, 48, TextureFormat.ARGB32, false)
        {
            name = name,
            filterMode = FilterMode.Point,
        };

        Sprite sprite = Sprite.CreateSprite(
            texture,
            new Rect(0, 0, 48, 48),
            new Vector2(0.5f, 0.5f),
            1,
            1,
            SpriteMeshType.FullRect,
            new Vector4(0, 0, 0, 0),
            true,
            []
        );

        sprite.name = name;

        return sprite;
    }
}
