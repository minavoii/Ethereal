

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Ethereal.Classes.Settings;

public class ExtendedSettingsSnapshot
{
    /// <summary>
    /// Additional snapshot values for custom settings
    /// </summary>
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

public static class SettingsSnapshotExtensions
{
    private static readonly ConditionalWeakTable<SettingsSnapshot, ExtendedSettingsSnapshot> _extra
        = new();

    public static ExtendedSettingsSnapshot Extra(this SettingsSnapshot snapshot)
        => _extra.GetOrCreateValue(snapshot);
}