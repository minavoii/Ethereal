using HarmonyLib;

namespace Ethereal.Patches;

internal static class MonsterState
{
    /// <summary>
    /// Required for custom actions to work, as they should not be `enabled`
    /// at creation time to prevent Unity's `Update()` calls.
    /// </summary>
    /// <param name="action"></param>
    [HarmonyPatch(typeof(BaseAction), "StartAction")]
    [HarmonyPrefix]
    private static void Prefix(BaseAction __instance)
    {
        if (!CombatStateManager.Instance.IsPreview)
        {
            __instance.enabled = true;

            if (__instance.GetComponent<VFX>() is VFX vfx)
                vfx.enabled = true;
        }
    }
}
