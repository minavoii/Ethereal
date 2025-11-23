

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ethereal.Classes.Settings;

public static class GameSettingsControllerExtensions
{
    private static readonly ConditionalWeakTable<GameSettingsController, ExtendedGameSettingsController> _extra
        = new();

    public static ExtendedGameSettingsController Extension(this GameSettingsController controller)
        => _extra.GetOrCreateValue(controller);

    public static T GetCustom<T>(this GameSettingsController controller, string setting)
        => _extra.GetOrCreateValue(controller).Get<T>(setting);
}

public class ExtendedGameSettingsController
{
    public Dictionary<string, object> CustomSettings { get; } = new();

    public T Get<T>(string setting)
    {
        if (CustomSettings.TryGetValue(setting, out object value))
        {
            if (value is T typedValue)
            {
                return typedValue;
            }
            else
            {
                Debug.LogError($"Custom setting {setting} is not of type {typeof(T)}.");
                return default;
            }
        }
        else
        {
            Debug.LogError($"Custom setting {setting} does not have stored value.");
            return default;
        }
    }

    public void Set(string setting, object value)
    {
        CustomSettings[setting] = value;
    }
}