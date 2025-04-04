using UnityEngine;

namespace Ethereal.API;

public static class Icon
{
    public enum IconType
    {
        Equipment,
        Action,
        ActionSmall,
        ActionCutSmall,
        Trait,
        MonsterType,
    }

    public static Sprite LoadFromAsset(string path, string asset)
    {
        GameObject gameObject = Utils.Assets.LoadAsset(path, asset);
        Sprite sprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        return sprite;
    }

    public static Sprite LoadFromImage(IconType iconType, string path)
    {
        if (!System.IO.File.Exists(path))
        {
            Log.API.LogWarning("File not found: " + path);
            return null;
        }

        Sprite sprite = CreateBySize(
            iconType switch
            {
                IconType.Equipment or IconType.Trait => (48, 48),
                IconType.Action => (18, 18),
                IconType.ActionSmall => (12, 12),
                IconType.ActionCutSmall => (12, 8),
                IconType.MonsterType => (7, 7),
                _ => (0, 0),
            }
        );

        sprite.texture.LoadImage(System.IO.File.ReadAllBytes(path));

        return sprite;
    }

    private static Sprite CreateBySize((int width, int height) size)
    {
        Texture2D texture = new(size.width, size.height, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point,
        };

        Sprite sprite = Sprite.CreateSprite(
            texture,
            new Rect(0, 0, size.width, size.height),
            new Vector2(0.5f, 0.5f),
            1,
            1,
            SpriteMeshType.FullRect,
            new Vector4(0, 0, 0, 0),
            true,
            []
        );

        return sprite;
    }
}
