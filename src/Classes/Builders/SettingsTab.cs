
namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record to define a new settings tab
/// </summary>
/// <param name="Name"></param>
/// <param name="MaxItemsPerColumn"></param>
/// <param name="FixedItemPositions"></param>
/// <param name="PlacementMode"></param>
/// <param name="VariableSize"></param>
/// <param name="HasPaging"></param>
/// <param name="MaxItemsPerPage"></param>
public sealed record SettingsTabBuilder(
    string Name,
    int MaxItemsPerColumn = 5,
    bool FixedItemPositions = true,
    MenuList.ItemPlacementMode PlacementMode = MenuList.ItemPlacementMode.Default,
    bool VariableSize = false,
    bool HasPaging = false,
    int MaxItemsPerPage = 0
);
