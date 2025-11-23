using System.Collections.Generic;
using System.Linq;
using Ethereal.Attributes;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Ethereal.API;

public interface ICustomSetting { }
public class SettingsTabBuilder
{
    public string Name { get; set; } = "";
    public int MaxItemsPerColumn { get; set; } = 5;
    public bool FixedItemPositions { get; set; } = true;
    public MenuList.ItemPlacementMode PlacementMode { get; set; } = MenuList.ItemPlacementMode.Default;
    public bool VariableSize { get; set; }
    public bool HasPaging { get; set; }
    public int MaxItemsPerPage { get; set; }
}

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
        GameObject pageObject = menu.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(m => m.name == "Page_Accessibility").gameObject;
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
        GameObject headerObject = menu.GetComponentsInChildren<PagingHeader>(true)
            .FirstOrDefault(m => m.name == $"Header_Accessibility").gameObject;
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

        // Inject into SettingsMenu
        menu.Pages = [.. menu.Pages, newHeader];
        menu.RevertChangesButtons = [.. menu.RevertChangesButtons, newPageObject.GetComponentsInChildren<MenuListItem>().First(menu => menu.gameObject.name == "MenuItem_Revert")];
    }

    public static void FixTabHeaders()
    {
        Debug.Log($"Fixing Tab headers");
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
}
