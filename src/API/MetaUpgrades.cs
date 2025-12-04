using System.Collections.Generic;
using System.Linq;
using Ethereal.Attributes;
using Ethereal.CustomFlags;
using UnityEngine;
using static MetaUpgradeDialogueEventManager;

namespace Ethereal.API;

[Deferrable]
public static partial class MetaUpgrades
{
    /// <summary>
    /// Add a meta upgrade to an npc dialogue's listing.
    /// </summary>
    /// <param name="npc"></param>
    /// <param name="page"></param>
    /// <param name="upgrade"></param>
    [Deferrable]
    private static void AddToNPC_Impl(EMetaUpgradeNPC npc, string page, MetaUpgrade upgrade)
    {
        List<MetaUpgradeDialogueEventManager> dialogues =
        [
            .. Resources.FindObjectsOfTypeAll<MetaUpgradeDialogueEventManager>(),
        ];

        string dialogueName = $"NPC_{npc}_MetaUpgrades";

        if (
            dialogues.FirstOrDefault(x => x?.name == dialogueName)
                is MetaUpgradeDialogueEventManager dialogue
            && dialogue.AvailableUpgrades.FirstOrDefault(x => x?.CustomHeader == page)
                is MetaUpgradePageData pageData
        )
        {
            pageData.AvailableUpgrades.Add(upgrade);
        }
    }

    /// <summary>
    /// Cleans up all added meta upgrades
    /// </summary>
    public static void Cleanup()
    {
        List<MetaUpgradeDialogueEventManager> dialogues =
        [
            .. Resources.FindObjectsOfTypeAll<MetaUpgradeDialogueEventManager>(),
        ];

        foreach (var dialogue in dialogues)
        {
            foreach (var pageData in dialogue.AvailableUpgrades)
            {
                List<MetaUpgrade> upgrades = pageData.AvailableUpgrades
                    .Where(u => u.gameObject.IsCustomObject())
                    .ToList();

                foreach (var upgrade in upgrades)
                {
                    pageData.AvailableUpgrades.Remove(upgrade);
                }
            }
        }
    }
}
