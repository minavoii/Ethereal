using System.Linq;
using TMPro;
using UnityEngine;

namespace Ethereal.Classes.Settings;

public class SimpleSnapshotConverter<T> : ISnapshotConverter<T, T>
{
    public T ToSetting(T data) => data;
    public T ToSnapshot(T data) => data;
}

public class SimpleBoolWriter(string key) : IPrefWriter<bool>
{
    public string Key { get; } = key;

    public void StoreToPrefs(bool data)
    {
        PlayerPrefsManager.SetInt(Key, data ? 1 : 0);
    }
}

public class SimpleBoolReader(string key) : IPrefReader<bool>
{
    public string Key { get; } = key;
    public bool ReadSettingFromPrefs()
    {
        return PlayerPrefsManager.GetInt(Key) > 0;
    }
}

public class BooleanCustomSetting : BasicCustomSetting<bool, bool>
{
    public string Key { get; set; }
    public System.Func<bool> IsEnabled { get; set; } = () => true;
    private MenuListItemToggle? _control { get; set; }
    public float? WidthOverride { get; set; }
    public override float Height => 33;

    public BooleanCustomSetting(
        string tab,
        string name,
        string description,
        string key,
        bool defaultValue
    ) : base(tab, name, description, defaultValue, new SimpleSnapshotConverter<bool>(), new SimpleBoolWriter(key), new SimpleBoolReader(key))
    {
        Key = key;
    }

    public override void ApplySnapshot(ExtendedSettingsSnapshot snapshot)
    {
        if (!IsEnabled())
        {
            return;
        }

        base.ApplySnapshot(snapshot);
    }

    public override MenuListItem BuildControl(SettingsMenu menu)
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
            GameSettingsController.Instance.Extension().Set(Name, SnapshotConverter.ToSetting(value));
            PrefWriter.StoreToPrefs(value);
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

    public override void UpdateControlState()
    {
        _control?.SetState(GameSettingsController.Instance.GetCustom<bool>(Name), shouldFireEvent: false);
        _control?.SetDisabled(!IsEnabled());
    }
}