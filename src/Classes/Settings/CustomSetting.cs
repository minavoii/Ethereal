

namespace Ethereal.Classes.Settings;

/// <summary>
/// Basic custom setting implementation
/// </summary>
public interface ICustomSetting
{
    /// <summary>
    /// Settings Tab that this custom setting should be added to.
    /// </summary>
    public string Tab { get; }
    /// <summary>
    /// Name of the setting
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Description of the setting
    /// </summary>
    public string Description { get; }
    /// <summary>
    /// Height of the control. Used for auto-positioning
    /// </summary>
    public float Height { get; }
    /// <summary>
    /// Override the X coordinate position
    /// </summary>
    public float? PositionOverrideX { get; }
    /// <summary>
    /// Override the Y coordinate position
    /// </summary>
    public float? PositionOverrideY { get; }
    /// <summary>
    /// Stores the default setting snapshot value
    /// Used in the reverting settings to default workflow
    /// </summary>
    /// <param name="defaultSettings"></param>
    public void SetDefaultSnapshot(ExtendedSettingsSnapshot defaultSettings);
    /// <summary>
    /// Stores the current setting snapshot value
    /// Used in the revert changes workflow
    /// </summary>
    /// <param name="rollbackSnapshot"></param>
    public void SetRollbackSnapshot(ExtendedSettingsSnapshot rollbackSnapshot);
    /// <summary>
    /// Applies the stored snapshot value to the setting
    /// </summary>
    /// <param name="snapshot"></param>
    public void ApplySnapshot(ExtendedSettingsSnapshot snapshot);
    /// <summary>
    /// Initializes the setting value from player prefs
    /// </summary>
    /// <param name="controller"></param>
    public void InitializeValue(ExtendedGameSettingsController controller);
    /// <summary>
    /// Creates the MenuListItem control for the setting.
    /// </summary>
    /// <param name="menu"></param>
    /// <returns></returns>
    public MenuListItem BuildControl(SettingsMenu menu);
    /// <summary>
    /// Refreshes the control state
    /// </summary>
    public void UpdateControlState();
}