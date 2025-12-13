using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Ethereal.Attributes;
using Ethereal.Classes.View;
using UnityEngine;

namespace Ethereal.API;

[BasicAPI]
public static partial class Sprites
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

    internal static Dictionary<int, Sprite[]> ShiftedSprites { get; } = [];

    /// <summary>
    /// Mark the API as ready and run all deferred methods.
    /// </summary>
    internal static async Task SetReady()
    {
        await BulkReplaceFromDefaultDirectory();

        API.SetReady();
    }

    /// <summary>
    /// Load a sprite from an asset bundle.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static Sprite? LoadFromBundle(string path, string asset) =>
        Assets.LoadBundle(path) is AssetBundle bundle ? LoadFromBundle(bundle, asset) : null;

    /// <summary>
    /// Load a sprite from an asset bundle.
    /// </summary>
    /// <param name="bundle"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static Sprite? LoadFromBundle(AssetBundle bundle, string asset)
    {
        GameObject? go = Assets.LoadPrefab(bundle, asset);
        Sprite? sprite = go?.GetComponent<SpriteRenderer>().sprite;

        return sprite;
    }

    /// <summary>
    /// Load all sprites from an asset bundle.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Sprite[] LoadAllFromBundle(string path) =>
        Assets.LoadBundle(path) is AssetBundle bundle ? LoadAllFromBundle(bundle) : [];

    /// <summary>
    /// Load all sprites from an asset bundle.
    /// </summary>
    /// <param name="bundle"></param>
    /// <returns></returns>
    public static Sprite[] LoadAllFromBundle(AssetBundle bundle) => bundle.LoadAllAssets<Sprite>();

    /// <summary>
    /// Create a sprite from an image file.
    /// </summary>
    /// <param name="iconType"></param>
    /// <param name="path"></param>
    /// <returns>a Sprite if the file was found; otherwise null.</returns>
    public static Sprite? LoadFromImage(SpriteType iconType, string path)
    {
        if (!File.Exists(path))
        {
            Log.API.LogError("File not found: " + path);
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

    private static async Task BulkReplaceFromDefaultDirectory()
    {
        Directory.CreateDirectory(SpritesPath);

        // Load from directory
        await BulkReplaceFromDirectory(SpritesPath);

        // Load bundles
        foreach (FileInfo file in new DirectoryInfo(SpritesPath).EnumerateFiles())
            await BulkReplaceFromBundle(file.FullName);
    }

    /// <summary>
    /// Replace sprites in bulk from images within a directory.<para/>
    /// Inside it, Action sprites need to be stored within an `Actions` folder,
    /// artifact sprites within an `Artifacts` folder, etc.
    /// </summary>
    /// <param name="spritesPath"></param>
    public static async Task BulkReplaceFromDirectory(string spritesPath)
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
        try
        {
            foreach (FileInfo file in new DirectoryInfo(PathActions).EnumerateFiles())
                await ReplaceIconAction(ToTitleCase(file.Name), null, file.FullName);
        }
        catch (Exception e)
            when (e is DirectoryNotFoundException || e is System.Security.SecurityException) { }

        // Artifacts
        try
        {
            foreach (FileInfo file in new DirectoryInfo(PathArtifacts).EnumerateFiles())
                await ReplaceIconArtifact(ToTitleCase(file.Name), null, file.FullName);
        }
        catch (Exception e)
            when (e is DirectoryNotFoundException || e is System.Security.SecurityException) { }

        // Buffs
        try
        {
            foreach (FileInfo file in new DirectoryInfo(PathBuffs).EnumerateFiles())
                if (LoadFromImage(SpriteType.Buff, file.FullName) is Sprite icon)
                    await ReplaceIconBuff(ToTitleCase(file.Name), icon);
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
                if (LoadFromImage(SpriteType.Equipment, file.FullName) is Sprite icon)
                    await ReplaceIconEquipment(ToTitleCase(file.Name), icon);
        }
        catch (Exception e)
            when (e is DirectoryNotFoundException || e is System.Security.SecurityException) { }

        // Mementos
        try
        {
            foreach (FileInfo file in new DirectoryInfo(PathMementos).EnumerateFiles())
                if (LoadFromImage(SpriteType.Memento, file.FullName) is Sprite icon)
                    await ReplaceIconMemento(ToTitleCase(file.Name), icon);
        }
        catch (Exception e)
            when (e is DirectoryNotFoundException || e is System.Security.SecurityException) { }

        // Traits
        try
        {
            foreach (FileInfo file in new DirectoryInfo(PathTraits).EnumerateFiles())
                if (LoadFromImage(SpriteType.Trait, file.FullName) is Sprite icon)
                    await ReplaceIconTrait(ToTitleCase(file.Name), icon);
        }
        catch (Exception e)
            when (e is DirectoryNotFoundException || e is System.Security.SecurityException) { }

        // Types
        try
        {
            foreach (FileInfo file in new DirectoryInfo(PathTypes).EnumerateFiles())
                if (LoadFromImage(SpriteType.MonsterType, file.FullName) is Sprite icon)
                    await ReplaceIconType(ToTitleCase(file.Name), icon);
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
    public static async Task BulkReplaceFromBundle(string path)
    {
        AssetBundle? bundle = AssetBundle.LoadFromFile(path);

        if (bundle is not null)
        {
            foreach (string assetName in bundle.GetAllAssetNames())
            {
                Texture2D? asset = Assets.LoadAsset<Texture2D>(bundle, assetName);

                if (asset is null)
                    continue;

                string dir = Path.GetDirectoryName(assetName)[7..];
                string name = ToTitleCase(assetName);
                Sprite icon = CreateBySize((asset.width, asset.height), asset);

                if (dir == "actions")
                    await ReplaceIconAction(name, icon);
                else if (dir == "artifacts")
                    await ReplaceIconArtifact(name, icon);
                else if (dir == "buffs")
                    await ReplaceIconBuff(name, icon);
                else if (dir == "elements")
                    ReplaceIconElement(name, icon);
                else if (dir == "equipments")
                    await ReplaceIconEquipment(name, icon);
                else if (dir == "mementos")
                    await ReplaceIconMemento(name, icon);
                else if (dir == "traits")
                    await ReplaceIconTrait(name, icon);
                else if (dir == "types")
                    await ReplaceIconType(name, icon);
            }
        }
    }

    /// <summary>
    /// Replace an action's icon with the given sprite or image file.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    /// <param name="iconPath"></param>
    private static async Task ReplaceIconAction(
        string name,
        Sprite? icon = null,
        string iconPath = ""
    )
    {
        if (name.EndsWith("Small"))
        {
            string actionName = name[..(name.Length - 6)];

            if (await Actions.Get(actionName) is BaseAction action)
                action.ActionIconSmall = icon ?? LoadFromImage(SpriteType.ActionSmall, iconPath);
        }
        else if (name.EndsWith("Cut"))
        {
            string actionName = name[..(name.Length - 4)];

            if (await Actions.Get(actionName) is BaseAction action)
                action.ActionIconCutSmall =
                    icon ?? LoadFromImage(SpriteType.ActionCutSmall, iconPath);
        }
        else
        {
            if (await Actions.Get(name) is BaseAction action)
            {
                Sprite? sprite = icon ?? LoadFromImage(SpriteType.Action, iconPath);

                action.ActionIconBig = sprite;
                action.Icon = sprite;
            }
        }
    }

    /// <summary>
    /// Replace an artifact's icon with the given sprite or image file.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    /// <param name="iconPath"></param>
    private static async Task ReplaceIconArtifact(
        string name,
        Sprite? icon = null,
        string iconPath = ""
    )
    {
        if (name.EndsWith("Big"))
        {
            string artifactName = name[..(name.Length - 4)];

            if (await Artifacts.Get(artifactName) is Consumable artifact)
                artifact.ActionIconBig = icon ?? LoadFromImage(SpriteType.Action, iconPath);
        }
        else if (name.EndsWith("Small"))
        {
            string artifactName = name[..(name.Length - 6)];

            if (await Artifacts.Get(artifactName) is Consumable artifact)
                artifact.ActionIconSmall = icon ?? LoadFromImage(SpriteType.Action, iconPath);
        }
        else if (await Artifacts.Get(name) is Consumable artifact)
            artifact.Icon = icon ?? LoadFromImage(SpriteType.Artifact, iconPath);
    }

    /// <summary>
    /// Replace a buff's icon with the given sprite.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    private static async Task ReplaceIconBuff(string name, Sprite icon)
    {
        if (await Buffs.Get(name) is Buff buff)
            buff.MonsterHUDIconSmall = icon;
    }

    /// <summary>
    /// Replace an element's icon with the given sprite or image file.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    /// <param name="iconPath"></param>
    private static void ReplaceIconElement(string name, Sprite? icon = null, string iconPath = "")
    {
        if (name.EndsWith("Empty"))
        {
            string elementName = name[..(name.Length - 6)];

            new ElementView(Enum.Parse<EElement>(elementName))
            {
                IconSmallEmpty = icon ?? LoadFromImage(SpriteType.ElementSmall, iconPath),
            };
        }
        else if (name.EndsWith("Filled"))
        {
            string elementName = name[..(name.Length - 7)];

            new ElementView(Enum.Parse<EElement>(elementName))
            {
                IconSmallFilled = icon ?? LoadFromImage(SpriteType.ElementSmall, iconPath),
            };
        }
        else
        {
            new ElementView(Enum.Parse<EElement>(name))
            {
                Icon = icon ?? LoadFromImage(SpriteType.ElementSmall, iconPath),
            };
        }
    }

    /// <summary>
    /// Replace an equipment's icon with the given sprite.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    private static async Task ReplaceIconEquipment(string name, Sprite icon)
    {
        string equipmentName = name;
        ERarity rarity = ERarity.Common;

        if (name.EndsWith("Epic"))
        {
            equipmentName = name[..(name.Length - 5)];
            rarity = ERarity.Epic;
        }
        else if (name.EndsWith("Rare"))
        {
            equipmentName = name[..(name.Length - 5)];
            rarity = ERarity.Rare;
        }

        if (await Equipments.Get(equipmentName, rarity) is Equipment equipment)
            equipment.Icon = icon;
    }

    /// <summary>
    /// Replace a memento's icon with the given sprite.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    private static async Task ReplaceIconMemento(string name, Sprite icon)
    {
        if (!name.EndsWith("Memento"))
            name += " Memento";

        if (await Mementos.Get(name) is MonsterMemento memento)
            memento.Icon = icon;
    }

    /// <summary>
    /// Replace a trait's icon with the given sprite.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    private static async Task ReplaceIconTrait(string name, Sprite icon)
    {
        if (await Traits.Get(name) is Trait trait)
            trait.Icon = icon;
    }

    /// <summary>
    /// Replace a monster type's icon with the given sprite.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    private static async Task ReplaceIconType(string name, Sprite icon)
    {
        if (await MonsterTypes.Get(Enum.Parse<EMonsterType>(name)) is MonsterType type)
            type.TypeIcon = icon;
    }

    /// <summary>
    /// Replace a string's underscores with spaces and turn it into TitleCase.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    private static string ToTitleCase(string filename) =>
        new CultureInfo("en-US").TextInfo.ToTitleCase(
            Path.GetFileNameWithoutExtension(filename).Replace("_", " ")
        );

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
