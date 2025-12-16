using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Attributes;
using UnityEngine;
using static MetaUpgradeDialogueEventManager;

namespace Ethereal.API;

[BasicAPI]
public static partial class MetaUpgrades
{
    private static List<MetaUpgradeDialogueEventManager> MetaUpgradeDialogues { get; set; } = [];

    /// <summary>
    /// Mark the API as ready and run all deferred methods.
    /// </summary>
    internal static void SetReady()
    {
        MetaUpgradeDialogues =
        [
            .. Resources.FindObjectsOfTypeAll<MetaUpgradeDialogueEventManager>(),
        ];

        API.SetReady();
    }

    /// <summary>
    /// Add a meta upgrade to an npc dialogue's listing.
    /// </summary>
    /// <param name="npc"></param>
    /// <param name="page"></param>
    /// <param name="upgrade"></param>
    public static async Task AddToNPC(EMetaUpgradeNPC npc, string page, MetaUpgrade upgrade)
    {
        await WhenReady();

        string dialogueName = $"NPC_{npc}_MetaUpgrades";

        if (
            MetaUpgradeDialogues.FirstOrDefault(x => x?.name == dialogueName)
                is MetaUpgradeDialogueEventManager dialogue
            && dialogue.AvailableUpgrades.FirstOrDefault(x => x?.CustomHeader == page)
                is MetaUpgradePageData pageData
        )
        {
            pageData.AvailableUpgrades.Add(upgrade);
        }
    }
}
