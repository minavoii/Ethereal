using HarmonyLib;

namespace Ethereal.Patches;

internal static class Projectiles
{
    /// <summary>
    /// Required for custom projectiles.
    /// Custom projectiles are disabled initially to avoid calling updates
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPatch(typeof(Projectile), "Initialize")]
    [HarmonyPrefix]
    private static void Prefix(Projectile __instance)
    {
        __instance.gameObject.SetActive(true);
    }
}
