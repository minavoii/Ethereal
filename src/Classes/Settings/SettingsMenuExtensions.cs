using System.Collections.Generic;
using HarmonyLib;

namespace Ethereal.Classes.Settings;


public static class SettingsMenuExtensions
{
    public static void EnableRevertSettings(this SettingsMenu menu)
    {
        int currentPageIndex = Traverse.Create(menu)
                               .Field("currentPageIndex")
                               .GetValue<int>();

        Traverse.Create(menu)
            .Method("EnableRevertSettings", currentPageIndex)
            .GetValue();
    }

    public static void OpenSelectionMenu(this SettingsMenu menu, List<string> entries, MenuList.ItemSelectedFunction callback, int maxItemsPerColumn = 10)
    {
        menu.SubSelectionList.MaxItemsPerColumn = maxItemsPerColumn;
        var tr = Traverse.Create(menu);
        tr.Field("currentSubSelectMethod").SetValue(callback);
        menu.OpenSubSelection(entries);
    }
}