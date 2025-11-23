using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Ethereal.Classes.Settings;

public class SimpleIntWriter(string key) : IPrefWriter<int>
{
    public string Key { get; } = key;

    public void StoreToPrefs(int data)
    {
        PlayerPrefsManager.SetInt(Key, data);
    }
}

public class SelectConverter<T>(List<SelectEntry<T>> entries) : ISnapshotConverter<T, int>
{
    public List<SelectEntry<T>> Entries { get; } = entries;

    public T ToSetting(int data)
    {
        return Entries[data].Value;
    }

    public int ToSnapshot(T data)
    {
        return Entries.FindIndex(entry => entry.Value?.Equals(data) ?? false);
    }
}

public class SelectionReader<T>(string key, List<SelectEntry<T>> entries) : IPrefReader<T>
{
    public string Key { get; } = key;
    public List<SelectEntry<T>> Entries { get; } = entries;
    public T ReadSettingFromPrefs()
    {
        int index = PlayerPrefsManager.GetInt(Key);
        return Entries[index].Value;
    }
}

public record SelectEntry<T>(string Name, T Value);

public class SelectCustomSetting<T> : BasicCustomSetting<T, int> where T : notnull
{
    public string Key { get; set; }
    private MenuListItem? _control { get; set; }
    private TextMeshPro? _currentText { get; set; }
    public override float Height => 33;
    public List<SelectEntry<T>> Entries;
    public SelectCustomSetting(
        string tab,
        string name,
        string description,
        string key,
        List<SelectEntry<T>> entries,
        int defaultValue = 0
    ) : base(tab, name, description, defaultValue, new SelectConverter<T>(entries), new SimpleIntWriter(key), new SelectionReader<T>(key, entries))
    {
        Key = key;
        Entries = entries;
    }

    public override MenuListItem BuildControl(SettingsMenu menu)
    {
        MenuListItem menuItem = menu.GetComponentsInChildren<MenuListItem>(true)
            .FirstOrDefault(m => m.name == "MenuItem_ChangeLanguage");

        GameObject newGameObject = Object.Instantiate(menuItem.gameObject);
        newGameObject.name = $"MenuItem_{Name}";
        MenuListItem newControl = newGameObject.GetComponent<MenuListItem>();
        newControl.ItemDescription = Description;
        newControl.InputOptions[0].Description = Description;

        GameObject nameTextObj = newGameObject.transform.Find("ItemText").gameObject;
        TextMeshPro nameText = nameTextObj.GetComponent<TextMeshPro>();
        nameText.text = Name;

        GameObject selectionTextObj = newGameObject.transform.Find("CurrentLanguageText").gameObject;
        selectionTextObj.name = $"SelectText_{Name}";

        newControl.OnItemSelected.AddListener(delegate
        {
            menu.OpenSelectionMenu(Entries.Select(e => e.Name).ToList(), (MenuListItem selecteditem) =>
            {
                GameSettingsController.Instance.Extension().Set(Name, SnapshotConverter.ToSetting(menu.SubSelectionList.CurrentIndex));
                PrefWriter.StoreToPrefs(menu.SubSelectionList.CurrentIndex);
                UpdateControlState();
                menu.EnableRevertSettings();
            });
        });

        _control = newControl;
        _currentText = selectionTextObj.GetComponent<TextMeshPro>();
        return newControl;
    }

    public override void UpdateControlState()
    {
        int index = SnapshotConverter.ToSnapshot(GameSettingsController.Instance.GetCustom<T>(Name));
        string current = Entries[index].Name;
        _currentText!.text = current;
    }
}