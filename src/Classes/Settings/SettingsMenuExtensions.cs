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
}