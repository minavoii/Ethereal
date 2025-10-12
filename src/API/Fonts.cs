using System.Collections.Generic;
using System.IO;
using Ethereal.Generator;
using TMPro;
using UnityEngine;

namespace Ethereal.API;

[Deferreable]
public static partial class Fonts
{
    internal static readonly List<TMP_FontAsset> CustomFonts = [];

    private static readonly string FontsPath = Path.Join(Plugin.EtherealPath, "Fonts");

    /// <summary>
    /// Load all custom fonts and mark the API as ready.
    /// </summary>
    internal static void SetReady()
    {
        LoadFonts();
        TMP_Settings.useModernHangulLineBreakingRules = true;

        API.SetReady();
    }

    /// <summary>
    /// Add a font as a fallback to another font.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="fallback"></param>
    [Deferreable]
    private static void AddFallback_Impl(TMP_FontAsset font, TMP_FontAsset fallback)
    {
        font.fallbackFontAssetTable.Add(fallback);
    }

    /// <summary>
    /// Add all custom fonts as a fallback to another font.
    /// </summary>
    /// <param name="font"></param>
    [Deferreable]
    private static void AddAllFallbacks_Impl(TMP_FontAsset font)
    {
        foreach (TMP_FontAsset fontAsset in CustomFonts)
            font.fallbackFontAssetTable.Add(fontAsset);
    }

    /// <summary>
    /// Load fonts files from the Ethereal directory.
    /// </summary>
    private static void LoadFonts()
    {
        Directory.CreateDirectory(FontsPath);

        foreach (FileInfo file in new DirectoryInfo(FontsPath).EnumerateFiles())
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(file.FullName);

            foreach (string name in bundle.GetAllAssetNames())
                CustomFonts.Add(bundle.LoadAsset<TMP_FontAsset>(name));
        }
    }
}
