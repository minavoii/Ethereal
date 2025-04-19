using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Ethereal.API;

public static class Sprites
{
    public enum SpriteType
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

    private static readonly string SpritesPath = Path.Join(Plugin.EtherealPath, "Sprites");

    private static readonly ConcurrentQueue<string> QueueBulkReplaceDirectory = new();

    private static readonly ConcurrentQueue<string> QueueBulkReplaceBundle = new();

    private static bool IsReady = false;

    /// <summary>
    /// Mark the API as ready and run all deferred methods.
    /// </summary>
    internal static void SetReady()
    {
        IsReady = true;

        BulkReplaceFromDefaultDirectory();

        while (QueueBulkReplaceDirectory.TryDequeue(out var path))
            BulkReplaceFromDirectory(path);

        while (QueueBulkReplaceBundle.TryDequeue(out var path))
            BulkReplaceFromBundle(path);
    }

    /// <summary>
    /// Load a sprite from an asset bundle.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static Sprite LoadFromAsset(string path, string asset)
    {
        GameObject gameObject = Utils.Assets.LoadAsset(path, asset);
        Sprite sprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        return sprite;
    }

    /// <summary>
    /// Create a sprite from an image file.
    /// </summary>
    /// <param name="iconType"></param>
    /// <param name="path"></param>
    /// <returns>a Sprite if the file was found; otherwise null.</returns>
    public static Sprite LoadFromImage(SpriteType iconType, string path)
    {
        if (!File.Exists(path))
        {
            Log.API.LogWarning("File not found: " + path);
            return null;
        }

        Sprite sprite = CreateBySize(
            iconType switch
            {
                SpriteType.Action => (18, 18),
                SpriteType.ActionSmall => (12, 12),
                SpriteType.ActionCutSmall => (12, 8),
                SpriteType.Artifact or SpriteType.Equipment or SpriteType.Trait => (48, 48),
                SpriteType.Element => (8, 8),
                SpriteType.ElementSmall => (4, 4),
                SpriteType.Memento => (38, 38),
                SpriteType.MonsterType or SpriteType.Buff => (7, 7),
                _ => (0, 0),
            }
        );

        sprite.texture.LoadImage(File.ReadAllBytes(path));

        return sprite;
    }

    private static void BulkReplaceFromDefaultDirectory()
    {
        Directory.CreateDirectory(SpritesPath);

        // Load from directory
        BulkReplaceFromDirectory(SpritesPath);

        // Load bundles
        foreach (FileInfo file in new DirectoryInfo(SpritesPath).EnumerateFiles())
            BulkReplaceFromBundle(file.FullName);
    }

    /// <summary>
    /// Replace sprites in bulk from images within a directory.<para/>
    /// Inside it, Action sprites need to be stored within an `Actions` folder,
    /// artifact sprites within an `Artifacts` folder, etc.
    /// </summary>
    /// <param name="spritesPath"></param>
    public static void BulkReplaceFromDirectory(string spritesPath)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueBulkReplaceDirectory.Enqueue(spritesPath);
            return;
        }

        string PathActions = Path.Join(spritesPath, "Actions");
        string PathArtifacts = Path.Join(spritesPath, "Artifacts");
        string PathBuffs = Path.Join(spritesPath, "Buffs");
        string PathElements = Path.Join(spritesPath, "Elements");
        string PathEquipments = Path.Join(spritesPath, "Equipments");
        string PathMementos = Path.Join(spritesPath, "Mementos");
        string PathTraits = Path.Join(spritesPath, "Traits");
        string PathTypes = Path.Join(spritesPath, "Types");

        // Actions
        try
        {
            foreach (FileInfo file in new DirectoryInfo(PathActions).EnumerateFiles())
                ReplaceIconAction(ToTitleCase(file.Name), null, file.FullName);
        }
        catch (Exception e)
            when (e is DirectoryNotFoundException || e is System.Security.SecurityException) { }

        // Artifacts
        try
        {
            foreach (FileInfo file in new DirectoryInfo(PathArtifacts).EnumerateFiles())
                ReplaceIconArtifact(ToTitleCase(file.Name), null, file.FullName);
        }
        catch (Exception e)
            when (e is DirectoryNotFoundException || e is System.Security.SecurityException) { }

        // Buffs
        try
        {
            foreach (FileInfo file in new DirectoryInfo(PathBuffs).EnumerateFiles())
                ReplaceIconBuff(
                    ToTitleCase(file.Name),
                    LoadFromImage(SpriteType.Buff, file.FullName)
                );
        }
        catch (Exception e)
            when (e is DirectoryNotFoundException || e is System.Security.SecurityException) { }

        // Elements
        try
        {
            foreach (FileInfo file in new DirectoryInfo(PathElements).EnumerateFiles())
                ReplaceIconElement(ToTitleCase(file.Name), null, file.FullName);
        }
        catch (Exception e)
            when (e is DirectoryNotFoundException || e is System.Security.SecurityException) { }

        // Equipments
        try
        {
            foreach (FileInfo file in new DirectoryInfo(PathEquipments).EnumerateFiles())
                ReplaceIconEquipment(
                    ToTitleCase(file.Name),
                    LoadFromImage(SpriteType.Equipment, file.FullName)
                );
        }
        catch (Exception e)
            when (e is DirectoryNotFoundException || e is System.Security.SecurityException) { }

        // Mementos
        try
        {
            foreach (FileInfo file in new DirectoryInfo(PathMementos).EnumerateFiles())
                ReplaceIconMemento(
                    ToTitleCase(file.Name),
                    LoadFromImage(SpriteType.Memento, file.FullName)
                );
        }
        catch (Exception e)
            when (e is DirectoryNotFoundException || e is System.Security.SecurityException) { }

        // Traits
        try
        {
            foreach (FileInfo file in new DirectoryInfo(PathTraits).EnumerateFiles())
                ReplaceIconTrait(
                    ToTitleCase(file.Name),
                    LoadFromImage(SpriteType.Trait, file.FullName)
                );
        }
        catch (Exception e)
            when (e is DirectoryNotFoundException || e is System.Security.SecurityException) { }

        // Types
        try
        {
            foreach (FileInfo file in new DirectoryInfo(PathTypes).EnumerateFiles())
                ReplaceIconType(
                    ToTitleCase(file.Name),
                    LoadFromImage(SpriteType.MonsterType, file.FullName)
                );
        }
        catch (Exception e)
            when (e is DirectoryNotFoundException || e is System.Security.SecurityException) { }
    }

    /// <summary>
    /// Replace sprites in bulk from images within an asset bundle.<para/>
    /// Action sprites need to be stored within an `Actions` folder,
    /// artifact sprites within an `Artifacts` folder, etc.
    /// </summary>
    /// <param name="path"></param>
    public static void BulkReplaceFromBundle(string path)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueBulkReplaceBundle.Enqueue(path);
            return;
        }

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

    /// <summary>
    /// Replace an action's icon with the given sprite or image file.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    /// <param name="iconPath"></param>
    private static void ReplaceIconAction(string name, Sprite icon, string iconPath = "")
    {
        Actions.ActionDescriptor descriptor = new();

        if (name.EndsWith("Small"))
        {
            name = name[..(name.Length - 6)];
            descriptor.IconSmall = icon ?? LoadFromImage(SpriteType.ActionSmall, iconPath);
        }
        else if (name.EndsWith("Cut"))
        {
            name = name[..(name.Length - 4)];
            descriptor.IconCutSmall = icon ?? LoadFromImage(SpriteType.ActionCutSmall, iconPath);
        }
        else
            descriptor.Icon = icon ?? LoadFromImage(SpriteType.Action, iconPath);

        Actions.Update(name, descriptor);
    }

    /// <summary>
    /// Replace an artifact's icon with the given sprite or image file.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    /// <param name="iconPath"></param>
    private static void ReplaceIconArtifact(string name, Sprite icon, string iconPath = "")
    {
        Artifacts.ArtifactDescriptor descriptor = new();

        if (name.EndsWith("Big"))
        {
            name = name[..(name.Length - 4)];
            descriptor.ActionIconBig = icon ?? LoadFromImage(SpriteType.Action, iconPath);
        }
        else if (name.EndsWith("Small"))
        {
            name = name[..(name.Length - 6)];
            descriptor.ActionIconSmall = icon ?? LoadFromImage(SpriteType.ActionSmall, iconPath);
        }
        else
            descriptor.Icon = icon ?? LoadFromImage(SpriteType.Artifact, iconPath);

        Artifacts.Update(name, descriptor);
    }

    /// <summary>
    /// Replace a buff's icon with the given sprite.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    private static void ReplaceIconBuff(string name, Sprite icon)
    {
        Buffs.UpdateIcon(name, icon);
    }

    /// <summary>
    /// Replace an element's icon with the given sprite or image file.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    /// <param name="iconPath"></param>
    private static void ReplaceIconElement(string name, Sprite icon, string iconPath = "")
    {
        Elements.ElementDescriptor descriptor = new();

        if (name.EndsWith("Empty"))
        {
            name = name[..(name.Length - 6)];
            descriptor.IconSmallEmpty = icon ?? LoadFromImage(SpriteType.ElementSmall, iconPath);
        }
        else if (name.EndsWith("Filled"))
        {
            name = name[..(name.Length - 7)];
            descriptor.IconSmallFilled = icon ?? LoadFromImage(SpriteType.ElementSmall, iconPath);
        }
        else if (name.EndsWith("Normal"))
        {
            name = name[..(name.Length - 7)];
            descriptor.IconSmall = icon ?? LoadFromImage(SpriteType.ElementSmall, iconPath);
        }
        else
            descriptor.Icon = icon ?? LoadFromImage(SpriteType.Element, iconPath);

        Elements.Update(Enum.Parse<EElement>(name), descriptor);
    }

    /// <summary>
    /// Replace an equipment's icon with the given sprite.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
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

        Equipments.Update(name, rarity, new() { Icon = icon });
    }

    /// <summary>
    /// Replace a memento's icon with the given sprite.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    private static void ReplaceIconMemento(string name, Sprite icon)
    {
        if (!name.EndsWith("Memento"))
            name += " Memento";

        Mementos.UpdateIcon(name, icon);
    }

    /// <summary>
    /// Replace a trait's icon with the given sprite.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    private static void ReplaceIconTrait(string name, Sprite icon)
    {
        Traits.Update(name, new() { Icon = icon });
    }

    /// <summary>
    /// Replace a monster type's icon with the given sprite.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    private static void ReplaceIconType(string name, Sprite icon)
    {
        MonsterTypes.UpdateIcon(Enum.Parse<EMonsterType>(name), icon);
    }

    /// <summary>
    /// Replace a string's underscores with spaces and turn it into TitleCase.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    private static string ToTitleCase(string filename)
    {
        return new CultureInfo("en-US").TextInfo.ToTitleCase(
            Path.GetFileNameWithoutExtension(filename).Replace("_", " ")
        );
    }

    /// <summary>
    /// Create a sprite with a given size and load a texture inside.
    /// </summary>
    /// <param name="size"></param>
    /// <param name="texture"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Create a sprite with a given size and a default texture.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    private static Sprite CreateBySize((int width, int height) size)
    {
        Texture2D texture = new(size.width, size.height, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point,
        };

        return CreateBySize(size, texture);
    }
}
