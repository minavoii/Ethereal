using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace Ethereal.API;

public static class Fonts
{
    private static bool IsReady = false;

    private static readonly ConcurrentQueue<(
        TMP_FontAsset font,
        TMP_FontAsset fallback
    )> QueueAddFallback = new();

    internal static readonly List<TMP_FontAsset> CustomFonts = [];

    private static readonly string FontsPath = Path.Join(Plugin.EtherealPath, "Fonts");

    /// <summary>
    /// Mark the API as ready and run all deferred methods.
    /// </summary>
    public static void SetReady()
    {
        LoadFonts();
        TMP_Settings.useModernHangulLineBreakingRules = true;

        IsReady = true;

        while (QueueAddFallback.TryDequeue(out var item))
        {
            if (item.fallback != null)
                AddFallback(item.font, item.fallback);
            else
                AddAllFallbacks(item.font);
        }
    }

    /// <summary>
    /// Add a font as a fallback to another font.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="fallback"></param>
    public static void AddFallback(TMP_FontAsset font, TMP_FontAsset fallback)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueAddFallback.Enqueue((font, fallback));
            return;
        }

        font.fallbackFontAssetTable.Add(fallback);
    }

    /// <summary>
    /// Add all custom fonts as a fallback to another font.
    /// </summary>
    /// <param name="font"></param>
    public static void AddAllFallbacks(TMP_FontAsset font)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueAddFallback.Enqueue((font, null));
            return;
        }

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
