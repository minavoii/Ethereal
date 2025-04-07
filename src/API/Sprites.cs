using System;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Ethereal.API;

public static class Sprites
{
    public enum IconType
    {
        Action,
        ActionSmall,
        ActionCutSmall,
        Artifact,
        Buff,
        Element,
        ElementSmall,
        Equipment,
        Memento,
        MonsterType,
        Trait,
    }

    public static Sprite LoadFromAsset(string path, string asset)
    {
        GameObject gameObject = Utils.Assets.LoadAsset(path, asset);
        Sprite sprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        return sprite;
    }

    public static Sprite LoadFromImage(IconType iconType, string path)
    {
        if (!File.Exists(path))
        {
            Log.API.LogWarning("File not found: " + path);
            return null;
        }

        Sprite sprite = CreateBySize(
            iconType switch
            {
                IconType.Action => (18, 18),
                IconType.ActionSmall => (12, 12),
                IconType.ActionCutSmall => (12, 8),
                IconType.Artifact or IconType.Equipment or IconType.Trait => (48, 48),
                IconType.Element => (8, 8),
                IconType.ElementSmall => (4, 4),
                IconType.Memento => (38, 38),
                IconType.MonsterType or IconType.Buff => (7, 7),
                _ => (0, 0),
            }
        );

        sprite.texture.LoadImage(File.ReadAllBytes(path));

        return sprite;
    }

    public static void BulkReplaceFromDirectory(string spritesPath)
    {
        string PathActions = Path.Join(spritesPath, "Actions");
        string PathArtifacts = Path.Join(spritesPath, "Artifacts");
        string PathBuffs = Path.Join(spritesPath, "Buffs");
        string PathElements = Path.Join(spritesPath, "Elements");
        string PathEquipments = Path.Join(spritesPath, "Equipments");
        string PathMementos = Path.Join(spritesPath, "Mementos");
        string PathTraits = Path.Join(spritesPath, "Traits");
        string PathTypes = Path.Join(spritesPath, "Types");

        // Actions
        foreach (FileInfo file in new DirectoryInfo(PathActions).EnumerateFiles())
            ReplaceIconAction(ToTitleCase(file.Name), null, file.FullName);

        // Artifacts
        foreach (FileInfo file in new DirectoryInfo(PathArtifacts).EnumerateFiles())
            ReplaceIconArtifact(ToTitleCase(file.Name), null, file.FullName);

        // Buffs
        foreach (FileInfo file in new DirectoryInfo(PathBuffs).EnumerateFiles())
            ReplaceIconBuff(ToTitleCase(file.Name), LoadFromImage(IconType.Buff, file.FullName));

        // Elements
        foreach (FileInfo file in new DirectoryInfo(PathElements).EnumerateFiles())
            ReplaceIconElement(ToTitleCase(file.Name), null, file.FullName);

        // Equipments
        foreach (FileInfo file in new DirectoryInfo(PathEquipments).EnumerateFiles())
            ReplaceIconEquipment(
                ToTitleCase(file.Name),
                LoadFromImage(IconType.Equipment, file.FullName)
            );

        // Mementos
        foreach (FileInfo file in new DirectoryInfo(PathMementos).EnumerateFiles())
            ReplaceIconMemento(
                ToTitleCase(file.Name),
                LoadFromImage(IconType.Memento, file.FullName)
            );

        // Traits
        foreach (FileInfo file in new DirectoryInfo(PathTraits).EnumerateFiles())
            ReplaceIconTrait(ToTitleCase(file.Name), LoadFromImage(IconType.Trait, file.FullName));

        // Types
        foreach (FileInfo file in new DirectoryInfo(PathTypes).EnumerateFiles())
            ReplaceIconType(
                ToTitleCase(file.Name),
                LoadFromImage(IconType.MonsterType, file.FullName)
            );
    }

    public static void BulkReplaceFromBundle(string path)
    {
        var bundle = AssetBundle.LoadFromFile(path);

        foreach (var assetName in bundle.GetAllAssetNames())
        {
            Texture2D asset = bundle.LoadAsset<Texture2D>(assetName);

            if (asset == null)
            {
                Log.Plugin.LogError($"Could not load asset: {path}:{asset}");
                continue;
            }

            string dir = Path.GetDirectoryName(assetName)[7..];
            string name = ToTitleCase(assetName);
            Sprite icon = CreateBySize((asset.width, asset.height), asset);

            if (dir == "actions")
                ReplaceIconAction(name, icon);
            else if (dir == "artifacts")
                ReplaceIconArtifact(name, icon);
            else if (dir == "buffs")
                ReplaceIconBuff(name, icon);
            else if (dir == "elements")
                ReplaceIconElement(name, icon);
            else if (dir == "equipments")
                ReplaceIconEquipment(name, icon);
            else if (dir == "mementos")
                ReplaceIconMemento(name, icon);
            else if (dir == "traits")
                ReplaceIconTrait(name, icon);
            else if (dir == "types")
                ReplaceIconType(name, icon);
        }
    }

    private static void ReplaceIconAction(string name, Sprite icon, string iconPath = "")
    {
        Actions.ActionDescriptor descriptor = new();

        if (name.EndsWith("Small"))
        {
            name = name[..(name.Length - 6)];
            descriptor.iconSmall = icon ?? LoadFromImage(IconType.ActionSmall, iconPath);
        }
        else if (name.EndsWith("Cut"))
        {
            name = name[..(name.Length - 4)];
            descriptor.iconCutSmall = icon ?? LoadFromImage(IconType.ActionCutSmall, iconPath);
        }
        else
            descriptor.icon = icon ?? LoadFromImage(IconType.Action, iconPath);

        Actions.Update(name, descriptor);
    }

    private static void ReplaceIconArtifact(string name, Sprite icon, string iconPath = "")
    {
        Artifacts.ArtifactDescriptor descriptor = new();

        if (name.EndsWith("Big"))
        {
            name = name[..(name.Length - 4)];
            descriptor.actionIconBig = icon ?? LoadFromImage(IconType.Action, iconPath);
        }
        else if (name.EndsWith("Small"))
        {
            name = name[..(name.Length - 6)];
            descriptor.actionIconSmall = icon ?? LoadFromImage(IconType.ActionSmall, iconPath);
        }
        else
            descriptor.icon = icon ?? LoadFromImage(IconType.Artifact, iconPath);

        Artifacts.Update(name, descriptor);
    }

    private static void ReplaceIconBuff(string name, Sprite icon)
    {
        Buffs.UpdateIcon(name, icon);
    }

    private static void ReplaceIconElement(string name, Sprite icon, string iconPath = "")
    {
        Elements.ElementIcons elementIcons = new();

        if (name.EndsWith("Empty"))
        {
            name = name[..(name.Length - 6)];
            elementIcons.iconSmallEmpty = icon ?? LoadFromImage(IconType.ElementSmall, iconPath);
        }
        else if (name.EndsWith("Filled"))
        {
            name = name[..(name.Length - 7)];
            elementIcons.iconSmallFilled = icon ?? LoadFromImage(IconType.ElementSmall, iconPath);
        }
        else if (name.EndsWith("Normal"))
        {
            name = name[..(name.Length - 7)];
            elementIcons.iconSmall = icon ?? LoadFromImage(IconType.ElementSmall, iconPath);
        }
        else
            elementIcons.icon = icon ?? LoadFromImage(IconType.Element, iconPath);

        Elements.UpdateIcon(Enum.Parse<EElement>(name), elementIcons);
    }

    private static void ReplaceIconEquipment(string name, Sprite icon)
    {
        ERarity rarity = ERarity.Common;

        if (name.EndsWith("Epic"))
        {
            name = name[..(name.Length - 5)];
            rarity = ERarity.Epic;
        }
        else if (name.EndsWith("Rare"))
        {
            name = name[..(name.Length - 5)];
            rarity = ERarity.Rare;
        }

        Equipments.Update(name, rarity, new() { icon = icon });
    }

    private static void ReplaceIconMemento(string name, Sprite icon)
    {
        if (!name.EndsWith("Memento"))
            name += " Memento";

        Mementos.UpdateIcon(name, icon);
    }

    private static void ReplaceIconTrait(string name, Sprite icon)
    {
        Trait.Update(name, new() { icon = icon });
    }

    private static void ReplaceIconType(string name, Sprite icon)
    {
        MonsterTypes.UpdateIcon(Enum.Parse<EMonsterType>(name), icon);
    }

    private static string ToTitleCase(string filename)
    {
        return new CultureInfo("en-US").TextInfo.ToTitleCase(
            Path.GetFileNameWithoutExtension(filename).Replace("_", " ")
        );
    }

    private static Sprite CreateBySize((int width, int height) size, Texture2D texture)
    {
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

    private static Sprite CreateBySize((int width, int height) size)
    {
        Texture2D texture = new(size.width, size.height, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point,
        };

        return CreateBySize(size, texture);
    }
}
