using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;
using Ethereal.Classes.Settings;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Ethereal.API;

[Deferrable]
public static partial class Settings
{
    internal static List<string> Tabs = [
        "General",
        "Input",
        "Audio",
        "Video",
        "Accessibility"
    ];

    private static SettingsMenu menu => Traverse.Create(UIController.Instance).Field("SettingsMenu").GetValue<SettingsMenu>();
    internal static List<ICustomSetting> CustomSettings { get; set; } = [];

    public class SettingsPageCache
    {
        public Dictionary<string, float> CachedPosition { get; set; } = new();
    }

    public static SettingsPageCache Cache { get; set; } = new();

    /// <summary>
    /// Mark the API as ready and run all deferred methods.
    /// </summary>
    internal static void SetReady()
    {
        InitializeCache();

        API.SetReady();
    }

    private static void InitializeCache()
    {
        Cache.CachedPosition["General"] = -199;
        Cache.CachedPosition["Input"] = -210;
        Cache.CachedPosition["Audio"] = -165;
        Cache.CachedPosition["Video"] = -110;
        Cache.CachedPosition["Accessibility"] = -165;
    }

    /// <summary>
    /// Add a new settings tab
    /// </summary>
    /// <param name="tabBuilder"></param>
    [Deferrable]
    private static void AddTab_Impl(SettingsTabBuilder tabBuilder)
    {
        int tabIndex = Tabs.Count;
        string page = tabBuilder.Name;
        Tabs.Add(page);

        // Adding page tab
        GameObject pageObject = GetTab("Accessibility").gameObject;
        GameObject newPageObject = Object.Instantiate(pageObject);
        newPageObject.transform.SetParent(pageObject.transform.parent, false);
        newPageObject.transform.SetSiblingIndex(tabIndex);

        newPageObject.name = $"Page_{page}";
        // Removing existing menu items from duplication
        int backItemIndex = newPageObject.GetComponentsInChildren<Transform>()
            .First(menu => menu.gameObject.name == "MenuItem_Back").GetSiblingIndex();
        for (int i = backItemIndex - 1; i >= 0; i--)
        {
            Object.DestroyImmediate(newPageObject.transform.GetChild(0).GetChild(i).gameObject);
        }
        MenuList menuList = newPageObject.GetComponent<MenuList>();
        menuList.MaxItemsPerColumn = tabBuilder.MaxItemsPerColumn;
        menuList.FixedItemPositions = tabBuilder.FixedItemPositions;
        menuList.PlacementMode = tabBuilder.PlacementMode;
        menuList.VariableSize = tabBuilder.VariableSize;
        menuList.HasPaging = tabBuilder.HasPaging;
        menuList.MaxItemsPerPage = tabBuilder.MaxItemsPerPage;

        // Adding header
        GameObject headerObject = GetTabHeader("Accessibility").gameObject;
        GameObject newHeaderObject = Object.Instantiate(headerObject);
        newHeaderObject.transform.SetParent(headerObject.gameObject.transform.parent, false);
        newHeaderObject.transform.SetSiblingIndex(tabIndex + 1);

        newHeaderObject.name = $"Header_{page}";
        PagingHeader newHeader = newHeaderObject.GetComponent<PagingHeader>();

        var tr = Traverse.Create(newHeader);
        tr.Field("pageContainer").SetValue(newPageObject);
        tr.Field("pageMenu").SetValue(newPageObject.GetComponent<MenuList>());
        tr.Field("pageMenuItems").SetValue(newPageObject.GetComponentsInChildren<MenuListItem>().ToArray());

        MouseEventHandler mouseEventHandler = newHeaderObject.GetComponent<MouseEventHandler>();
        for (int i = 0; i < mouseEventHandler.OnMouseClicked.GetPersistentEventCount(); i++)
        {
            mouseEventHandler.OnMouseClicked.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
        }
        mouseEventHandler.OnMouseClicked.AddListener(() =>
        {
            menu.OnPageIndexClicked(tabIndex);
        });

        TextMeshPro text = newHeaderObject.GetComponentInChildren<TextMeshPro>();
        text.text = page;

        Cache.CachedPosition[page] = 0;

        // Inject into SettingsMenu
        menu.Pages = [.. menu.Pages, newHeader];
        menu.RevertChangesButtons = [.. menu.RevertChangesButtons, newPageObject.GetComponentsInChildren<MenuListItem>().First(menu => menu.gameObject.name == "MenuItem_Revert")];
    }

    public static void FixTabHeaders()
    {
        // Fixing header widths
        int totalTabs = Tabs.Count;
        float currentSpacing = 95; // Width between headers
        float requiredSpacing = 5 * currentSpacing / totalTabs;
        float currentWidth = 85; // Width of labels
        float requiredWidth = 5 * currentWidth / totalTabs;
        float currentBorder = 96;
        float requiredBorder = 5 * currentBorder / totalTabs;
        for (int i = 0; i < totalTabs; i++)
        {
            PagingHeader header = GetTabHeader(Tabs[i]);
            header.transform.localPosition = new Vector3(30 + i * requiredSpacing, header.transform.localPosition.y, header.transform.localPosition.z);
            RectTransform labelTransform = header.transform.GetChild(0).transform as RectTransform;
            labelTransform.sizeDelta = new Vector2(requiredWidth, labelTransform.sizeDelta.y);
            var border = header.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();
            border.size = new Vector2(requiredBorder, border.size.y);
        }
    }

    private static PagingHeader GetTabHeader(string name)
    {
        return menu.GetComponentsInChildren<PagingHeader>(true)
            .FirstOrDefault(m => m.name == $"Header_{name}");
    }

    private static MenuList GetTab(string name)
    {
        return menu.GetComponentsInChildren<MenuList>(true)
            .FirstOrDefault(m => m.name == $"Page_{name}");
    }

    /// <summary>
    /// Add a new setting
    /// </summary>
    /// <param name="setting"></param>
    [Deferrable]
    private static void AddSetting_Impl(ICustomSetting setting)
    {
        CustomSettings.Add(setting);

        PagingHeader header = GetTabHeader(setting.Tab);
        MenuList tab = GetTab(setting.Tab);
        Transform root = tab.transform.GetChild(0);

        (MenuListItem newControl, float height) = setting.BuildControl(menu);

        newControl.transform.position = new Vector3(newControl.transform.position.x, Cache.CachedPosition[setting.Tab], newControl.transform.position.z);
        Cache.CachedPosition[setting.Tab] -= height;

        // Add to tab
        newControl.transform.SetParent(root, false);
        newControl.transform.SetSiblingIndex(root.childCount - 3);  // On top of back/revert/default

        // Add to header
        FieldInfo field = typeof(PagingHeader).GetField("pageMenuItems", BindingFlags.NonPublic | BindingFlags.Instance);
        List<MenuListItem> oldArray = (field.GetValue(header) as MenuListItem[]).ToList();
        oldArray.Insert(oldArray.Count - 3, newControl);
        field.SetValue(header, oldArray.ToArray());

        // Setting up default value
        setting.SetDefaultSnapshot(menu.defaultSettings.Extra());

        // Initializing value
        setting.InitializeValue(GameSettingsController.Instance.Extension());
    }
}
