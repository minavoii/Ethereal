using System.Linq;
using TMPro;
using UnityEngine;

namespace Ethereal.Classes.Settings;

/// <summary>
/// Basic implementation of a custom setting that stores only one value
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IBasicCustomSetting<T> : ICustomSetting
{
    /// <summary>
    /// Key to store the custom setting
    /// </summary>
    public string Key { get; set; }
    /// <summary>
    /// Default value for the setting
    /// </summary>
    public T DefaultValue { get; set; }
}

public class BooleanCustomSetting : IBasicCustomSetting<bool>
{
    public string Tab { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Key { get; set; }
    public bool DefaultValue { get; set; }
    public System.Func<bool> IsEnabled { get; set; } = () => true;
    private MenuListItemToggle? _control { get; set; }
    public float? WidthOverride { get; set; }
    public float? PositionOverrideX { get; set; }
    public float? PositionOverrideY { get; set; }
    public float Height => 33;

    public BooleanCustomSetting(
        string tab,
        string name,
        string description,
        string key,
        bool defaultValue
    )
    {
        Tab = tab;
        Name = name;
        Description = description;
        Key = key;
        DefaultValue = defaultValue;
    }

    public void SetDefaultSnapshot(ExtendedSettingsSnapshot defaultSettings)
    {
        defaultSettings.CustomSettings.Add(Name, DefaultValue);
    }
    public void SetRollbackSnapshot(ExtendedSettingsSnapshot rollbackSnapshot)
    {
        rollbackSnapshot.CustomSettings.Add(Name, GameSettingsController.Instance.GetCustom<bool>(Name));
    }
    public void ApplySnapshot(ExtendedSettingsSnapshot snapshot)
    {
        if (!IsEnabled())
        {
            return;
        }

        bool newValue = (bool)snapshot.CustomSettings[Name];
        SetValue(GameSettingsController.Instance.Extension(), newValue);
        UpdateControlState();
    }
    public void InitializeValue(ExtendedGameSettingsController controller)
    {
        controller.Set(Name, PlayerPrefsManager.GetInt(Key) > 0);
    }

    public void SetValue(ExtendedGameSettingsController controller, bool value)
    {
        controller.Set(Name, value);
        PlayerPrefsManager.SetInt(Key, value ? 1 : 0);
    }

    public MenuListItem BuildControl(SettingsMenu menu)
    {
        MenuListItemToggle menuItem = menu.GetComponentsInChildren<MenuListItemToggle>(true)
            .FirstOrDefault(m => m.name == "MenuItem_ColorblindAether");

        GameObject newGameObject = Object.Instantiate(menuItem.gameObject);
        newGameObject.name = $"MenuItem_{Name}";
        MenuListItemToggle newToggle = newGameObject.GetComponent<MenuListItemToggle>();
        newToggle.ItemDescription = Description;

        TextMeshPro text = newGameObject.GetComponentInChildren<TextMeshPro>();
        text.text = Name;

        // Disable all persistent listeners
        for (int i = 0; i < newToggle.OnToggle.GetPersistentEventCount(); i++)
        {
            newToggle.OnToggle.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
        }

        newToggle.OnToggle.AddListener((value) =>
        {
            SetValue(GameSettingsController.Instance.Extension(), value);
            menu.EnableRevertSettings();
        });
        newToggle.OnToggle.AddListener((value) =>
        {
            newGameObject.GetComponent<WwiseSFX>().PlayEventByName("Play_SFX_menu_toggle");
        });

        if (WidthOverride != null)
        {
            newToggle.ItemSize = new(WidthOverride.Value, newToggle.ItemSize.y);
            newToggle.ItemOffset = new((WidthOverride.Value - 5) / 2, 0);
        }

        _control = newToggle;
        return newToggle;
    }

    public void UpdateControlState()
    {
        _control?.SetState(GameSettingsController.Instance.GetCustom<bool>(Name), shouldFireEvent: false);
        _control?.SetDisabled(!IsEnabled());
    }
}