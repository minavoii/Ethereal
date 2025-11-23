
namespace Ethereal.Classes.Settings;

public interface IPrefWriter<TSnapshot>
{
    void StoreToPrefs(TSnapshot data);
}

public interface IPrefReader<TSetting>
{
    TSetting ReadSettingFromPrefs();
}

public interface ISnapshotConverter<TSetting, TSnapshot>
{
    TSnapshot ToSnapshot(TSetting data);
    TSetting ToSetting(TSnapshot data);
}

/// <summary>
/// Basic implementation of a custom setting that stores only one value
/// </summary>
/// <typeparam name="TSnapshot">Type stored in the snapshot or from the control</typeparam>
/// <typeparam name="TData">Type stored in player prefs</typeparam>
public abstract class BasicCustomSetting<TSetting, TSnapshot> : ICustomSetting where TSnapshot : notnull where TSetting : notnull
{
    public string Tab { get; }
    public string Name { get; }
    public string Description { get; }
    public abstract float Height { get; }
    public float? PositionOverrideX { get; set; }
    public float? PositionOverrideY { get; set; }
    public TSnapshot DefaultValue { get; }

    protected ISnapshotConverter<TSetting, TSnapshot> SnapshotConverter;
    protected IPrefWriter<TSnapshot> PrefWriter;
    protected IPrefReader<TSetting> PrefReader;

    public BasicCustomSetting(
        string tab,
        string name,
        string description,
        TSnapshot defaultValue,
        ISnapshotConverter<TSetting, TSnapshot> snapshotConverter,
        IPrefWriter<TSnapshot> prefWriter,
        IPrefReader<TSetting> prefReader
    )
    {
        Tab = tab;
        Name = name;
        Description = description;
        DefaultValue = defaultValue;
        SnapshotConverter = snapshotConverter;
        PrefWriter = prefWriter;
        PrefReader = prefReader;
    }

    public void SetDefaultSnapshot(ExtendedSettingsSnapshot defaultSettings)
    {
        defaultSettings.CustomSettings.Add(Name, DefaultValue);
    }
    public void SetRollbackSnapshot(ExtendedSettingsSnapshot rollbackSnapshot)
    {
        rollbackSnapshot.CustomSettings.Add(Name, SnapshotConverter.ToSnapshot(GameSettingsController.Instance.GetCustom<TSetting>(Name)));
    }
    public virtual void ApplySnapshot(ExtendedSettingsSnapshot snapshot)
    {
        TSnapshot newValue = (TSnapshot)snapshot.CustomSettings[Name];
        GameSettingsController.Instance.Extension().Set(Name, SnapshotConverter.ToSetting(newValue));
        PrefWriter.StoreToPrefs(newValue);
        UpdateControlState();
    }

    public abstract MenuListItem BuildControl(SettingsMenu menu);

    public void InitializeValue(ExtendedGameSettingsController controller)
    {
        controller.Set(Name, PrefReader.ReadSettingFromPrefs());
    }

    public abstract void UpdateControlState();
}